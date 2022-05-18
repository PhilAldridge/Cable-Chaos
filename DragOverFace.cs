using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DragOverFace : MonoBehaviour
{
    public List<GameObject> connectionsStart;
    public string faceType = "path"; //other options "source", "disabled"
    public TMP_Text LinesConnected = null;
    public TMP_Text roundScore = null;
    public GameObject solvedPanel = null;
    public GameObject pipePrefab = null;
    public GameObject bigcube = null;
    private Color color;
    private GameObject oppositeSource;
    private float roundStart;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log(cubeScript.cubeDrag);
        ZPlayerPrefs.Initialize("what'sYourName", "salt12issalt");
        LinesConnected.text = "0/" + Create3x3cube.CGpaths.Count.ToString();
        roundStart = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnMouseDown()
    {
        foreach(List<GameObject> path in Create3x3cube.HumanPaths)
        {
            //if source or part of path
            if (path[0] == this.gameObject || (path.Contains(this.gameObject) && faceType =="path"))
            {
                cubeScript.cubeDrag = true;
                cubeScript.selectedPath = path;
                int thisPathIndex = Create3x3cube.HumanPaths.IndexOf(path);
                cubeScript.selectedPipes = Create3x3cube.Pipes[thisPathIndex];
                cubeScript.pathIndex = (int)Mathf.Floor((float)thisPathIndex /2.0f);



                if (thisPathIndex%2 == 0)
                {
                    cubeScript.otherPathIndex = thisPathIndex + 1;
                }
                else
                {
                    cubeScript.otherPathIndex = thisPathIndex - 1;
                }
                
                cubeScript.currentIndex = path.IndexOf(this.gameObject);
            }    
        }

        
        //GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        //foreach (GameObject connectedFace in connectionsStart)
        /*{
            connectedFace.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
        }*/

    }

    void OnMouseUp()
    {
        cubeScript.cubeDrag = false;
        /*GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        foreach (GameObject connectedFace in connectionsStart)
        {
            connectedFace.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            
        }*/
    }

    void OnMouseOver()
    {
        //if dragging from path to a connected face that is a path
        if (cubeScript.cubeDrag && cubeScript.selectedPath[cubeScript.currentIndex].GetComponent<DragOverFace>().connectionsStart.Contains(this.gameObject) && faceType == "path")
        {
            //delete loops if dragging pack to earlier part of the path (like an idiot!)
            if (cubeScript.selectedPath.Contains(this.gameObject))
            {
                cubeScript.currentIndex = cubeScript.selectedPath.IndexOf(this.gameObject) - 1;
                //TODO delete pipes
                Debug.Log("1");
            }

            //unattach later faces in the path            
            while (cubeScript.currentIndex + 1 < cubeScript.selectedPath.Count)
            {
                /*if (cubeScript.selectedPath[cubeScript.currentIndex + 1].GetComponent<DragOverFace>().faceType == "path")
                {
                    cubeScript.selectedPath[cubeScript.currentIndex + 1].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
                }*/
                cubeScript.selectedPath.RemoveAt(cubeScript.currentIndex + 1);
                Debug.Log("2");
            }

            //delete later pipes in the path
            while (cubeScript.currentIndex + 1 < cubeScript.selectedPipes.Count)
            {
                Debug.Log("yo");
                Destroy(cubeScript.selectedPipes[cubeScript.currentIndex + 1]);
                cubeScript.selectedPipes.RemoveAt(cubeScript.currentIndex + 1);
            }




            //if dragging when other source already has a path remove other path
            List<GameObject> otherPath = Create3x3cube.HumanPaths[cubeScript.otherPathIndex];
            while (otherPath.Count > 1)
            {
                /*if (otherPath[1].GetComponent<DragOverFace>().faceType == "path")
                {
                    otherPath[1].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
                }*/
                otherPath.RemoveAt(1);
                
            }

            //remove other pipes
            List<GameObject> otherPipes = Create3x3cube.Pipes[cubeScript.otherPathIndex];
            while (otherPipes.Count > 1)
            {
                Destroy(otherPipes[1]);
                otherPipes.RemoveAt(1);
                
            }




            //delete paths of other colours if we go through it.
            foreach (List<GameObject> path in Create3x3cube.HumanPaths)
            {
                if (path != cubeScript.selectedPath && path.Contains(this.gameObject))
                {
                    int i = path.IndexOf(this.gameObject);
                    int pathIndex = Create3x3cube.HumanPaths.IndexOf(path);
                    while (path.Count > i)
                    {
                        if(path[i].GetComponent<DragOverFace>().faceType == "path")
                        {
                            path[i].GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
                        }
                        Destroy(Create3x3cube.Pipes[pathIndex][i]);
                        Create3x3cube.Pipes[pathIndex].RemoveAt(i);
                        path.RemoveAt(i);
                        
                    }

               
                }
            }

            //add pipe
            GameObject newpipe = Instantiate(pipePrefab, bigcube.transform.position, bigcube.transform.rotation, bigcube.transform);
            newpipe.GetComponent<pipeScript>().Resize(transform.localPosition+transform.parent.transform.localPosition, cubeScript.selectedPath[cubeScript.selectedPath.Count - 1].transform.localPosition + cubeScript.selectedPath[cubeScript.selectedPath.Count - 1].transform.parent.transform.localPosition);
            newpipe.SetActive(true);
            cubeScript.selectedPipes.Add(newpipe);
            ColorUtility.TryParseHtmlString("#" + Create3x3cube.ColourValues[cubeScript.pathIndex], out color);
            foreach (MeshRenderer mesh in newpipe.GetComponentsInChildren<MeshRenderer>())
            {
                mesh.material.SetColor("_EmissionColor", color);
                mesh.material.SetColor("_Color", color);
            }
            //add this face to the path
            cubeScript.selectedPath.Add(this.gameObject);
            //this.gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", cubeScript.selectedPath[0].GetComponent<MeshRenderer>().material.color);
            cubeScript.currentIndex++;




            //check if this new face touches the endpoint
            oppositeSource = Create3x3cube.HumanPaths[cubeScript.otherPathIndex][0];
            foreach (GameObject face in connectionsStart)
            {
                if(face.GetComponent<DragOverFace>().faceType == "source" && face != cubeScript.selectedPath[0] && face==oppositeSource)
                {
                    //connect the endpoint to the path and finish journey
                    cubeScript.selectedPath.Add(face);
                    cubeScript.cubeDrag = false;
                    newpipe = Instantiate(pipePrefab, bigcube.transform.position, bigcube.transform.rotation, bigcube.transform);
                    newpipe.GetComponent<pipeScript>().Resize(transform.localPosition + transform.parent.transform.localPosition, face.transform.localPosition + face.transform.parent.transform.localPosition);
                    newpipe.SetActive(true);
                    cubeScript.selectedPipes.Add(newpipe);
                    foreach (MeshRenderer mesh in newpipe.GetComponentsInChildren<MeshRenderer>())
                    {
                        mesh.material.SetColor("_EmissionColor", color);
                        mesh.material.SetColor("_Color", color);
                    }


                    //check for puzzle solved
                    int i = 0;
                    foreach(List<GameObject> path in Create3x3cube.HumanPaths)
                    {
                        if(path.Count>1 && path[path.Count-1].GetComponent<DragOverFace>().faceType == "source")
                        {
                            i++;
                        }
                    }
                    
                    LinesConnected.text = i.ToString() + "/" + Create3x3cube.CGpaths.Count.ToString();
                    if (i == Create3x3cube.CGpaths.Count)
                    {
                        solvedPanel.SetActive(true);
                        int thisScore = Mathf.RoundToInt(Create3x3cube.levelDifficulty * 1000f / (Time.time - roundStart));
                        int avgScore = thisScore;
                        string shapeName = Create3x3cube.shapeName;
                        if (ZPlayerPrefs.HasKey(shapeName))
                        {
                            avgScore = Mathf.RoundToInt(ZPlayerPrefs.GetInt(shapeName) * 0.9f + 0.1f * thisScore);
                            
                        } 

                        ZPlayerPrefs.SetInt(shapeName, avgScore);

                        roundScore.text = "Score: #" + thisScore.ToString() + "\n\nYour average score on " + shapeName + " is " + avgScore.ToString(); ;

                        if(ZPlayerPrefs.GetString("gameMode")== "Ranked")
                        {
                            ZPlayerPrefs.SetInt("puzzlesCompleted", ZPlayerPrefs.GetInt("puzzlesCompleted") + 1);
                        }
                    }
                    break;
                }
            }
             
        }
    }

    /*void OnMouseExit()
    {
        if (cubeScript.cubeDrag)
        {
            //GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        }
    }*/

    
}
