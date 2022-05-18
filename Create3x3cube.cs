using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Create3x3cube : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject prefab;
    public GameObject[,,] blocks;
    public GameObject SourcePrefab;
    public Material defaultMaterial;
    public static int levelDifficulty;
    private GameObject top, bottom, front, back, left, right;
    private List<GameObject> topConnections, bottomConnections, frontConnections, backConnections, leftConnections, rightConnections, faces, facesUnchecked, path1, pipe1, nextStepOptions, connectionsFromLast;

    public static int faceCount = 0;
    public static List<List<GameObject>> CGpaths;
    public static List<List<GameObject>> HumanPaths;
    public static List<List<GameObject>> Pipes;
    public static string shapeName;
    public int randSeed = 42;
    

    public static string[] ColourValues = new string[]
    {
            "808080","556b2f","7f0000","483d8b","008000","008b8b","4682b4","000080","9acd32","daa520","7f007f","8fbc8f","b03060","ff0000","ffff00","7cfc00","deb887","8a2be2","00ff7f","dc143c","00ffff",
            "0000ff","ff7f50","ff00ff","1e90ff","dda0dd","90ee90","add8e6","ff1493","7b68ee"    
    };
    
    private Color color = Color.red;
    private string[] shapeNames = new string[] { "Cuboid", "Staircase", "Wedge", "Pyramid", "Prism", "Octahedron", /*"Toroid", */"Hourglass", "Skeleton", "Random" };


    void Start()
    {
        ZPlayerPrefs.Initialize("what'sYourName", "salt12issalt");
        faceCount = 0;
        CGpaths = new List<List<GameObject>>();
        HumanPaths = new List<List<GameObject>>();
        Pipes = new List<List<GameObject>>();
        //define size of cube, positioning and 3x3 array to store each block.
        //byte cubeSize = 3;
        //float offset = (cubeSize-1)/2.0f;

        int length = 3;
        int width = 3;
        int height = 3;
        shapeName = "Random";
        if (ZPlayerPrefs.HasKey("length"))
        {
            
            length = ZPlayerPrefs.GetInt("length", 3);
            width = ZPlayerPrefs.GetInt("width", 3);
            height = ZPlayerPrefs.GetInt("height", 3);
            shapeName = shapeNames[ZPlayerPrefs.GetInt("shape", 0)];
        }
        
        
        if (shapeName == "Random")
        {
            shapeName = shapeNames[Random.Range(0, shapeNames.Length - 1)];
        }

        if (ZPlayerPrefs.GetString("gameMode", "Casual") == "Ranked")
        {
            float maxSize = 7f;
            int rank = 200;
            if (ZPlayerPrefs.HasKey("rating"))
            {
                rank = ZPlayerPrefs.GetInt("rating", 200);
            }
            float wantedDifficulty = (1f + 5f * Random.value) * rank / 100;
            //removed skeleton as its too hard
            shapeName = shapeNames[Random.Range(0, shapeNames.Length - 2)];
            
            length = Random.Range(Mathf.RoundToInt(Mathf.Max(1f, Mathf.Ceil(wantedDifficulty / maxSize - maxSize))), Mathf.RoundToInt(Mathf.Min(maxSize, Mathf.Floor((wantedDifficulty - 1f) / 2f))));
            width = Random.Range(Mathf.RoundToInt(Mathf.Max(1f, Mathf.Ceil((wantedDifficulty - length * maxSize) / (length + maxSize)))), Mathf.RoundToInt(Mathf.Min(maxSize, Mathf.Floor((wantedDifficulty - length) / (length + 1)))));
            height = Mathf.RoundToInt(Mathf.Max(2f,Mathf.Min(maxSize, Mathf.Ceil((wantedDifficulty + length * width) / (width + length)))));
            levelDifficulty = length * width + length * height + width * height;
            if (ZPlayerPrefs.HasKey("totalDifficulty")) {
                ZPlayerPrefs.SetInt("totalDifficulty", ZPlayerPrefs.GetInt("totalDifficulty") + levelDifficulty);
            } else
            {
                ZPlayerPrefs.SetInt("totalDifficulty", levelDifficulty);
            }
       

        }

        levelDifficulty = length * width + length * height + width * height;


        float xoffset = (length - 1) / 2.0f;
        float yoffset = (width - 1) / 2.0f;
        float zoffset = (height - 1) / 2.0f;
        blocks = new GameObject[length, width, height];

        //create blocks from prefab and store them in blocks variable
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (shapeIncludesBlock(i, j, k, shapeName, length, width, height)) {
                        blocks[i, j, k] = Instantiate(prefab, new Vector3(i - xoffset, j - yoffset, k - zoffset), Quaternion.identity, transform);
                    } else
                    {
                        blocks[i, j, k] = null;
                    }
                }

            }
        }

        //tester
        //Destroy(blocks[0, 0, 0]);
        //blocks[0, 0, 0] = null;

        //create link array for each face of each block (COMPLICATED!)
        faces = new List<GameObject>();
        for (int i = 0; i < length; i++)
        {
            for (int j = 0; j < width; j++)
            {
                for (int k = 0; k < height; k++)
                {
                    if (blocks[i, j, k] == null) continue;
                    //get all the faces of the cube and their lists of connections
                    top = blocks[i, j, k].GetComponent<prefabScript>().top;
                    topConnections = top.GetComponent<DragOverFace>().connectionsStart;
                    bottom = blocks[i, j, k].GetComponent<prefabScript>().bottom;
                    bottomConnections = bottom.GetComponent<DragOverFace>().connectionsStart;
                    left = blocks[i, j, k].GetComponent<prefabScript>().left;
                    leftConnections = left.GetComponent<DragOverFace>().connectionsStart;
                    right = blocks[i, j, k].GetComponent<prefabScript>().right;
                    rightConnections = right.GetComponent<DragOverFace>().connectionsStart;
                    front = blocks[i, j, k].GetComponent<prefabScript>().front;
                    frontConnections = front.GetComponent<DragOverFace>().connectionsStart;
                    back = blocks[i, j, k].GetComponent<prefabScript>().back;
                    backConnections = back.GetComponent<DragOverFace>().connectionsStart;
                    



                    //top
                    if (j==(width-1) || blocks[i,j+1,k] == null) //if no block above it
                    {
                        faceCount++;
                        faces.Add(top);
                        top.GetComponent<Renderer>().enabled = true;
                        //front
                        if(i!=(length-1) && j!=(width-1) && blocks[i + 1, j+1, k] != null)
                        {
                            topConnections.Add(blocks[i + 1, j + 1, k].GetComponent<prefabScript>().back);
                        } else if(i==(length-1) || blocks[i+1,j,k] == null)  //if no block in front
                        {
                            topConnections.Add(front);
                        } else
                        {
                            topConnections.Add(blocks[i + 1, j, k].GetComponent<prefabScript>().top);
                        }

                        //back
                        if (i != 0 && j != (width - 1) && blocks[i - 1, j+1, k] != null)
                        {
                            topConnections.Add(blocks[i - 1, j + 1, k].GetComponent<prefabScript>().front);
                        }
                        else if (i == 0 || blocks[i - 1, j, k] == null)
                        {
                            topConnections.Add(back);
                        }
                        else
                        {
                            topConnections.Add(blocks[i - 1, j, k].GetComponent<prefabScript>().top);
                        }

                        //left
                        if (k != (height - 1) && j != (width - 1) && blocks[i,j +1, k+1] != null)
                        {
                            topConnections.Add(blocks[i, j + 1, k+1].GetComponent<prefabScript>().right);
                        }
                        else if (k == (height - 1) || blocks[i, j, k+1] == null)
                        {
                            topConnections.Add(left);
                        }
                        else
                        {
                            topConnections.Add(blocks[i , j, k+1].GetComponent<prefabScript>().top);
                        }

                        //right
                        if (k != 0 && j != (width - 1) && blocks[i, j + 1, k - 1] != null)
                        {
                            topConnections.Add(blocks[i, j + 1, k - 1].GetComponent<prefabScript>().left);
                        }
                        else if (k == 0 || blocks[i, j, k - 1] == null)
                        {
                            topConnections.Add(right);
                        }
                        else
                        {
                            topConnections.Add(blocks[i, j, k - 1].GetComponent<prefabScript>().top);
                        }
                    }



                    //bottom
                    if (j == 0 || blocks[i, j - 1, k] == null) //if no block below it
                    {
                        faceCount++;
                        bottom.GetComponent<Renderer>().enabled = true;
                        faces.Add(bottom);
                        //front
                        if (i != (length - 1) && j != 0 && blocks[i + 1, j - 1, k] != null)
                        {
                            bottomConnections.Add(blocks[i + 1, j - 1, k].GetComponent<prefabScript>().back);
                        }
                        else if (i == (length - 1) || blocks[i + 1, j, k] == null)  //if no block in front
                        {
                            bottomConnections.Add(front);
                        }
                        else
                        {
                            bottomConnections.Add(blocks[i + 1, j, k].GetComponent<prefabScript>().bottom);
                        }

                        //back
                        if (i != 0 && j != 0 && blocks[i - 1, j - 1, k] != null)
                        {
                            bottomConnections.Add(blocks[i - 1, j - 1, k].GetComponent<prefabScript>().front);
                        }
                        else if (i == 0 || blocks[i - 1, j, k] == null)
                        {
                            bottomConnections.Add(back);
                        }
                        else
                        {
                            bottomConnections.Add(blocks[i - 1, j, k].GetComponent<prefabScript>().bottom);
                        }

                        //left
                        if (k != (height - 1) && j != 0 && blocks[i, j - 1, k + 1] != null)
                        {
                            bottomConnections.Add(blocks[i, j - 1, k + 1].GetComponent<prefabScript>().right);
                        }
                        else if (k == (height - 1) || blocks[i, j, k + 1] == null)
                        {
                            bottomConnections.Add(left);
                        }
                        else
                        {
                            bottomConnections.Add(blocks[i, j, k + 1].GetComponent<prefabScript>().bottom);
                        }

                        //right
                        if (k != 0 && j != 0 && blocks[i, j - 1, k - 1] != null)
                        {
                            bottomConnections.Add(blocks[i, j - 1, k - 1].GetComponent<prefabScript>().left);
                        }
                        else if (k == 0 || blocks[i, j, k - 1] == null)
                        {
                            bottomConnections.Add(right);
                        }
                        else
                        {
                            bottomConnections.Add(blocks[i, j, k - 1].GetComponent<prefabScript>().bottom);
                        }

                    }



                    //left
                    if (k == (height-1) || blocks[i, j, k+1] == null) //if no to left
                    {
                        faceCount++;
                        faces.Add(left);
                        left.GetComponent<Renderer>().enabled = true;
                        //front
                        if (i != (length - 1) && k != (height-1) && blocks[i + 1, j, k+1] != null)
                        {
                            leftConnections.Add(blocks[i + 1, j, k+1].GetComponent<prefabScript>().back);
                        }
                        else if (i == (length - 1) || blocks[i + 1, j, k] == null)  //if no block in front
                        {
                            leftConnections.Add(front);
                        }
                        else
                        {
                            leftConnections.Add(blocks[i + 1, j, k].GetComponent<prefabScript>().left);
                        }

                        //back
                        if (i != 0 && k != (height-1) && blocks[i - 1, j, k+1] != null)
                        {
                            leftConnections.Add(blocks[i - 1, j, k+1].GetComponent<prefabScript>().front);
                        }
                        else if (i == 0 || blocks[i - 1, j, k] == null)
                        {
                            leftConnections.Add(back);
                        }
                        else
                        {
                            leftConnections.Add(blocks[i - 1, j, k].GetComponent<prefabScript>().left);
                        }

                        //top
                        if (k != (height - 1) && j != (width-1) && blocks[i, j + 1, k + 1] != null)
                        {
                            leftConnections.Add(blocks[i, j + 1, k + 1].GetComponent<prefabScript>().bottom);
                        }
                        else if (j == (width - 1) || blocks[i, j+1, k] == null)
                        {
                            leftConnections.Add(top);
                        }
                        else
                        {
                            leftConnections.Add(blocks[i, j+1, k].GetComponent<prefabScript>().left);
                        }

                        //bottom
                        if (j != 0 && k != (height-1) && blocks[i, j - 1, k + 1] != null)
                        {
                            leftConnections.Add(blocks[i, j - 1, k + 1].GetComponent<prefabScript>().top);
                        }
                        else if (j == 0 || blocks[i, j-1, k] == null)
                        {
                            leftConnections.Add(bottom);
                        }
                        else
                        {
                            leftConnections.Add(blocks[i, j-1, k].GetComponent<prefabScript>().left);
                        }

                    }



                    //right
                    if (k == 0 || blocks[i, j, k - 1] == null) //if no to right
                    {
                        faceCount++;
                        faces.Add(right);
                        right.GetComponent<Renderer>().enabled = true;
                        //front
                        if (i != (length - 1) && k != 0 && blocks[i + 1, j, k - 1] != null)
                        {
                            rightConnections.Add(blocks[i + 1, j, k - 1].GetComponent<prefabScript>().back);
                        }
                        else if (i == (length - 1) || blocks[i + 1, j, k] == null)  //if no block in front
                        {
                            rightConnections.Add(front);
                        }
                        else
                        {
                            rightConnections.Add(blocks[i + 1, j, k].GetComponent<prefabScript>().right);
                        }

                        //back
                        if (i != 0 && k != 0 && blocks[i - 1, j, k - 1] != null)
                        {
                            rightConnections.Add(blocks[i - 1, j, k - 1].GetComponent<prefabScript>().front);
                        }
                        else if (i == 0 || blocks[i - 1, j, k] == null)
                        {
                            rightConnections.Add(back);
                        }
                        else
                        {
                            rightConnections.Add(blocks[i - 1, j, k].GetComponent<prefabScript>().right);
                        }

                        //top
                        if (k != 0 && j != (width - 1) && blocks[i, j + 1, k - 1] != null)
                        {
                            rightConnections.Add(blocks[i, j + 1, k - 1].GetComponent<prefabScript>().bottom/*right*/);
                        }
                        else if (j == (width - 1) || blocks[i, j + 1, k] == null)
                        {
                            rightConnections.Add(top);
                        }
                        else
                        {
                            rightConnections.Add(blocks[i, j + 1, k].GetComponent<prefabScript>().right);
                        }

                        //bottom
                        if (j != 0 && k != 0 && blocks[i, j - 1, k - 1] != null)
                        {
                            rightConnections.Add(blocks[i, j - 1, k - 1].GetComponent<prefabScript>().top/*right*/);
                        }
                        else if (j == 0 || blocks[i, j - 1, k] == null)
                        {
                            rightConnections.Add(bottom);
                        }
                        else
                        {
                            rightConnections.Add(blocks[i, j - 1, k].GetComponent<prefabScript>().right);
                        }

                    }




                    //front
                    if (i == (length - 1) || blocks[i + 1, j, k] == null) //if no block in front
                    {
                        faceCount++;
                        faces.Add(front);
                        front.GetComponent<Renderer>().enabled = true;
                        //top
                        if (i != (length - 1) && j != (width - 1) && blocks[i + 1, j + 1, k] != null)
                        {
                            frontConnections.Add(blocks[i + 1, j + 1, k].GetComponent<prefabScript>().bottom);
                        }
                        else if (j == (width - 1) || blocks[i, j + 1, k] == null)  //if no block in front
                        {
                            frontConnections.Add(top);
                        }
                        else
                        {
                            frontConnections.Add(blocks[i, j+1, k].GetComponent<prefabScript>().front);
                        }

                        //bottom
                        if (i != (length - 1) && j != 0 && blocks[i + 1, j - 1, k] != null)
                        {
                            frontConnections.Add(blocks[i + 1, j - 1, k].GetComponent<prefabScript>().top);
                        }
                        else if (j == 0 || blocks[i, j - 1, k] == null)  //if no block in front
                        {
                            frontConnections.Add(bottom);
                        }
                        else
                        {
                            frontConnections.Add(blocks[i, j-1, k].GetComponent<prefabScript>().front);
                        }

                        //left
                        if (i != (length - 1) && k != (height - 1) && blocks[i + 1, j, k+1] != null)
                        {
                            frontConnections.Add(blocks[i + 1, j, k+1].GetComponent<prefabScript>().right);
                        }
                        else if (k == (height - 1) || blocks[i, j, k+1] == null)  //if no block in front
                        {
                            frontConnections.Add(left);
                        }
                        else
                        {
                            frontConnections.Add(blocks[i, j, k+1].GetComponent<prefabScript>().front);
                        }

                        //right
                        if (i != (length - 1) && k != 0 && blocks[i + 1, j, k-1] != null)
                        {
                            frontConnections.Add(blocks[i + 1, j, k-1].GetComponent<prefabScript>().left);
                        }
                        else if (k == 0 || blocks[i, j, k-1] == null)  //if no block in front
                        {
                            frontConnections.Add(right);
                        }
                        else
                        {
                            frontConnections.Add(blocks[i, j, k-1].GetComponent<prefabScript>().front);
                        }

                    }



                    //back
                    if (i == 0 || blocks[i - 1, j, k] == null) //if no block in front
                    {
                        faceCount++;
                        faces.Add(back);
                        back.GetComponent<Renderer>().enabled = true;
                        //top
                        if (i != 0 && j != (width - 1) && blocks[i - 1, j + 1, k] != null)
                        {
                            backConnections.Add(blocks[i - 1, j + 1, k].GetComponent<prefabScript>().bottom);
                        }
                        else if (j == (width - 1) || blocks[i, j + 1, k] == null)  //if no block in front
                        {
                            backConnections.Add(top);
                        }
                        else
                        {
                            backConnections.Add(blocks[i, j + 1, k].GetComponent<prefabScript>().back);
                        }

                        //bottom
                        if (i != 0 && j != 0 && blocks[i - 1, j - 1, k] != null)
                        {
                            backConnections.Add(blocks[i - 1, j - 1, k].GetComponent<prefabScript>().top);
                        }
                        else if (j == 0 || blocks[i, j - 1, k] == null)  //if no block in front
                        {
                            backConnections.Add(bottom);
                        }
                        else
                        {
                            backConnections.Add(blocks[i, j - 1, k].GetComponent<prefabScript>().back);
                        }

                        //left
                        if (i != 0 && k != (height - 1) && blocks[i - 1, j, k + 1] != null)
                        {
                            backConnections.Add(blocks[i - 1, j, k + 1].GetComponent<prefabScript>().right);
                        }
                        else if (k == (height - 1) || blocks[i, j, k + 1] == null)  //if no block in front
                        {
                            backConnections.Add(left);
                        }
                        else
                        {
                            backConnections.Add(blocks[i, j, k + 1].GetComponent<prefabScript>().back);
                        }

                        //right
                        if (i != 0 && k != 0 && blocks[i - 1, j, k - 1] != null)
                        {
                            backConnections.Add(blocks[i - 1, j, k - 1].GetComponent<prefabScript>().left);
                        }
                        else if (k == 0 || blocks[i, j, k - 1] == null)  //if no block in front
                        {
                            backConnections.Add(right);
                        }
                        else
                        {
                            backConnections.Add(blocks[i, j, k - 1].GetComponent<prefabScript>().back);
                        }

                        
                    }
                }

            }
        }




        //Create paths
        //Setup lists needed
        //Random.InitState(randSeed); only use to do specific puzzle
        facesUnchecked = new List<GameObject>();
        CGpaths = new List<List<GameObject>>();
        nextStepOptions = new List<GameObject>();
        

        //Next path
        while (faces.Count > 0)
        {
            //pick new color
            ColorUtility.TryParseHtmlString("#" + ColourValues[CGpaths.Count % ColourValues.Length], out color);

            //add new list for the path in CG paths
            CGpaths.Add(new List<GameObject>());
            HumanPaths.Add(new List<GameObject>());
            path1 = CGpaths[CGpaths.Count - 1];
            Pipes.Add(new List<GameObject>());
            pipe1 = Pipes[Pipes.Count - 1];

            //pick random point not yet on a path. Add it to the new path and remove from list of options for new paths
            int rand = Random.Range(0, faces.Count - 1);
            path1.Add(faces[rand]);
            HumanPaths[HumanPaths.Count - 1].Add(faces[rand]);
            pipe1.Add(Instantiate(SourcePrefab, faces[rand].transform.position, faces[rand].transform.rotation, transform));
            pipe1[0].SetActive(true);
            foreach (MeshRenderer mesh in pipe1[0].GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material.SetColor("_Color", color);
                mesh.material.SetColor("_EmissionColor", color);
            }
            //faces[rand].GetComponent<MeshRenderer>().material.SetColor("_Color", color);
            faces[rand].GetComponent<DragOverFace>().faceType = "source";
            
            faces.RemoveAt(rand);
            

            //copy list of unpicked faces
            facesUnchecked.Clear();
            foreach (GameObject face in faces)
            {
                facesUnchecked.Add(face);
            }


            //make random path going as far from the original point as possible
            while (facesUnchecked.Count > 0)
            {
                connectionsFromLast = path1[path1.Count - 1].GetComponent<DragOverFace>().connectionsStart;
                nextStepOptions.Clear();

                foreach (GameObject face in connectionsFromLast)
                {
                    if (facesUnchecked.Contains(face))
                    {
                        nextStepOptions.Add(face);
                        facesUnchecked.Remove(face);
                    }
                }

                

                if (nextStepOptions.Count == 0)
                {
                    break;
                }

                rand = Random.Range(0, nextStepOptions.Count - 1);

                path1.Add(nextStepOptions[rand]);
                //nextStepOptions[rand].GetComponent<MeshRenderer>().material.SetColor("_Color", color);

                //remove neighbouring points from the possible path
                foreach(GameObject face in nextStepOptions)
                {
                    if(face!=nextStepOptions[rand])
                    {
                        List<GameObject> otherconnections = face.GetComponent<DragOverFace>().connectionsStart;

                        foreach(GameObject face2 in otherconnections)
                        {
                            if (facesUnchecked.Contains(face2))
                            {
                                facesUnchecked.Remove(face2);
                            }
                        }
                    }
                }
                faces.Remove(nextStepOptions[rand]);

            }

            path1[path1.Count-1].GetComponent<MeshRenderer>().material.SetColor("_Color", color);
            path1[path1.Count - 1].GetComponent<DragOverFace>().faceType = "source";
            

            //disable short paths
            if (path1.Count<3 || HumanPaths.Count ==0)
            {
              
                
                foreach(GameObject face in path1)
                {
                    face.GetComponent<MeshRenderer>().material = defaultMaterial;
                    face.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.black);

                    face.GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", Color.black);
                    face.GetComponent<DragOverFace>().faceType = "disabled";
                }
                foreach(GameObject pipe in pipe1)
                {
                    Destroy(pipe);
                }
                Pipes.Remove(pipe1);
                CGpaths.Remove(path1);
                HumanPaths.RemoveAt(HumanPaths.Count-1);
            }
            else
            {
                HumanPaths.Add(new List<GameObject>());
                HumanPaths[HumanPaths.Count - 1].Add(path1[path1.Count - 1]);
                //path1[path1.Count - 1].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);
                //path1[0].GetComponent<MeshRenderer>().material.SetColor("_EmissionColor", color);

                Pipes.Add(new List<GameObject>());
                pipe1 = Pipes[Pipes.Count - 1];
                pipe1.Add(Instantiate(SourcePrefab, path1[path1.Count - 1].transform.position, path1[path1.Count - 1].transform.rotation, transform));
                pipe1[0].SetActive(true);
                foreach (MeshRenderer mesh in pipe1[0].GetComponentsInChildren<MeshRenderer>())
                {
                    mesh.material.SetColor("_Color", color);
                    mesh.material.SetColor("_EmissionColor", color);
                }

                if(HumanPaths.Count ==1)
                {
                    facesUnchecked.Clear();
                    faces.Clear();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    bool shapeIncludesBlock(int i, int j, int k, string shapeName, int length, int width, int height)
    {
        if(length<3 || width <3 || height < 3)
        {
            return true;
        }

        float midLength = 2 * (float)i / ((float)length - 1) - 1;
        float midWidth = 2 * (float)j / ((float)width - 1) - 1;
        float midHeight = 2 * (float)k / ((float)height - 1) - 1;

        switch (shapeName)
        {
            case "Staircase":
                if (Mathf.Abs(midHeight + midLength + midWidth) <= 1f)
                {
                    return true;
                } else
                {
                    return false;
                }
            case "Wedge":
                if (midHeight + midLength + midWidth <= 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "Prism":
                if (midHeight + midLength <= 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "Octahedron":
                if (Mathf.Abs(midHeight) + Mathf.Abs(midLength) + Mathf.Abs( midWidth) <= 1.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "Toroid":
                if (i==0 || i==length-1 || j ==0 || j==width-1)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "Skeleton":
                byte counter = 0;
                if(i==0 || i>=length-1)
                {
                    counter++;
                }
                if(j==0 || j>=width-1)
                {
                    counter++;
                }
                if(k==0 || k>=height-1)
                {
                    counter++;
                }
                if (counter>1)
                {
                    
                    return true;
                }
                else
                {
                    return false;
                }
            case "Hourglass":
                if (Mathf.Abs(midHeight) + Mathf.Abs(midLength) - 2 * Mathf.Abs(midWidth) <= 0.5f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            case "Pyramid":
                if (Mathf.Abs(midHeight) + Mathf.Abs(midLength) - 2.0*(float)j/(float)(width-1) < 0.1f)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            default:
                return true;
        }
    }
}
