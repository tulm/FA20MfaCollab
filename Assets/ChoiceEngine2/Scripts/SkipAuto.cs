using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using UnityEngine.SceneManagement;

/* This script handles skip and auto functions */
public class SkipAuto : MonoBehaviour {

    public static SkipAuto core;

    void Awake() {
        core = core == null ? this : core;
    }

    [HideInInspector]
    public bool skip = false;

    [HideInInspector]
    public bool auto = false;

    private Coroutine skipRoutine;
    private Coroutine autoRoutine;

    private GameObject clickCatcher;

    //Saving visited nodes
    public void saveVisited() {
        
        loadVisited();

        visitedNodeSave vns = new visitedNodeSave();
        vns.vn = Database.core.visitedNodesGlobal;
        SaveManager.core.JsonSave(vns, "visited", ".nodes");

    }

    //Loading visited nodes
    public void loadVisited() {

        if (File.Exists (Application.persistentDataPath + "/visited.nodes")) {
            visitedNodeSave vns = new visitedNodeSave();
            SaveManager.core.JsonLoad(vns, "visited", ".nodes");

            foreach (VisitedNode vn in vns.vn) {

                if (Database.core.visitedNodesGlobal.Find(x => x.nodeID == vn.nodeID && x.levelIndex == vn.levelIndex) == null) {
                    Database.core.visitedNodesGlobal.Add(vn);
                }

            }

        }

    }

    void Start() {
        clickCatcher = LocalReferences.core.clickCatcher;

        var b = clickCatcher.GetComponent<Button>();
        b.onClick.RemoveAllListeners();
        b.onClick.AddListener(delegate { auto = false; skip = false; clickCatcher.SetActive(false); });

        loadVisited();
    }

    //Initiating skipping routine
    public void Skip() {
        clickCatcher.SetActive(true);
        var delay = Settings.core.settings.skipDelay;
        skip = true;

        if (skipRoutine != null) {
            StopCoroutine(skipRoutine);
        }

        skipRoutine = StartCoroutine(skipEnumerator(delay));
    }

    //Initiating the skipping routine
    public void Auto() {
        clickCatcher.SetActive(true);
        var delay = Settings.core.settings.autoDelay;
        auto = true;

        if (autoRoutine != null) {
            StopCoroutine(autoRoutine);
        }

        autoRoutine = StartCoroutine(autoEnumerator(delay));
    }

    //Stopping skipping routine
    public void StopSkip() {
        clickCatcher.SetActive(false);
        skip = false;
        auto = false;
    }

    //Stopping auto routine
    public void StopAuto() {
        clickCatcher.SetActive(false);
        auto = false;
        skip = false;
    }

    IEnumerator autoEnumerator(float delay) {

        while(auto) {

            //Making sure that the current node is not a choice
            var currentNode = LevelManager.core.getNodeById(LevelManager.core.currentNode);
            if (currentNode.nodeType != Node.nodeTypeEnum.Choice && currentNode.nodeType != Node.nodeTypeEnum.Both) {

                //Waiting for the node to load.
                NodeManager.core.processing = true;

                //Loading next node
                var nextNode = LevelManager.core.getNodeById(currentNode.nextNodeId);
                if (nextNode != null && nextNode.nextNodeId != "" && LevelManager.core.nodes.Find(x => x.id == nextNode.id && !nextNode.onNodeEnter.loadLevelConfig.loadLevel) != null) {
                    NodeManager.core.displayNode(nextNode.nextNodeRandom ? nextNode.randomNodes[UnityEngine.Random.Range(0, nextNode.randomNodes.Count)]: nextNode.id);
                } else {
                    StopAuto();
                    break;
                }
            
                //Time limit
                var start = Time.deltaTime;

                //The loop will stop if more than 3 seconds pass or the done is done processing
                while (NodeManager.core.processing) {

                    if ((Time.deltaTime - start) > 3f) {
                        StopAuto();
                        break;
                    }

                    yield return new WaitForEndOfFrame();
                }

            } else {
                StopAuto();
            }

            yield return new WaitForSeconds(delay);
        }

    }

    IEnumerator skipEnumerator(float delay) {

        while (skip) {

            //Making sure that the current node is not a choice
            var currentNode = LevelManager.core.getNodeById(LevelManager.core.currentNode);
            if (currentNode.nodeType != Node.nodeTypeEnum.Choice && currentNode.nodeType != Node.nodeTypeEnum.Both) {

                var nextNode = LevelManager.core.getNodeById(currentNode.nextNodeId);
                if (nextNode == null || (!Settings.core.settings.skipUnseen && Database.core.visitedNodesGlobal.Find(x => x.nodeID == nextNode.id && x.levelIndex == SceneManager.GetActiveScene().buildIndex) == null)) {
                    StopSkip();
                    break;
                }

                //Waiting for the node to load.
                NodeManager.core.processing = true;

                //Loading next node
                if (nextNode != null && nextNode.id != "" && LevelManager.core.nodes.Find(x => x.id == nextNode.id && !nextNode.onNodeEnter.loadLevelConfig.loadLevel) != null) {
                    NodeManager.core.displayNode(nextNode.nextNodeRandom ? nextNode.randomNodes[UnityEngine.Random.Range(0, nextNode.randomNodes.Count)]: nextNode.id);
                } else {
                    StopSkip();
                    break;
                }

                //Time limit
                var start = Time.deltaTime;

                //The loop will stop if more than 3 seconds pass or the done is done processing
                while (NodeManager.core.processing) {

                    if ((Time.deltaTime - start) > 3f) {
                        StopSkip();
                        break;
                    }

                    yield return new WaitForEndOfFrame();
                }
                
            } else {
                StopSkip();
            }

            yield return new WaitForSeconds(delay);
        }

    }

}

public class visitedNodeSave {
    public List<VisitedNode> vn;

}

// Code by Cination / Tsenkilidis Alexandros.