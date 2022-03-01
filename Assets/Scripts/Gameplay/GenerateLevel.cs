using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Grapplynth {
    public class GenerateLevel : MonoBehaviour {
        // object for spawn segment
        public Object spawnSegment;
        // track the ID of the current generator
        public int genID;
        public int parentGenID;
        // use static variables to generate the next id and track generators
        public static int nextGenID;
        public static List<GameObject> generatorsList;
        // track if this generator can fork or not
        bool canFork;
        // track the player object
        public GameObject player;
        // list of segment prefabs to use for the spawning process
        public List<GameObject> segmentPrefabs = new List<GameObject>();
        // x, y, z of this spawner
        int x;
        int y;
        int z;
        // random variable for generation
        private System.Random r;
        // number of segments this spawner has made (mainly used for debug purposes)
        static int numSegments;
        private int maxSimultaneousTurns;
        // angle of level generator
        int rotation;
        // track how many turns to make
        int lastTurn = 0;
        int turnThreshold = 10;
        int numTurns = 0;

        // class used for segment initialization
        private class SegVals {
            public SegmentScript segmentScript;
            public GameObject segment;
            public int segmentGenID;
            public int numHallways;
        }

        // stores the initial hallway
        List<SegVals> initialHallway = new List<SegVals>();
        bool isSpawningInitial;

        bool killed = false;

        // store all the current hallways in list, deleting old ones as needed
        private List<List<SegVals>> hallwayList;

        private void OnEnable() {
            EventManager.OnTurnCorner.AddListener(delegate{GenerateNextHallway();});
            EventManager.OnRestart.AddListener(ResetGenID);
        }

        private void OnDisable() {
            EventManager.OnTurnCorner.RemoveListener(delegate{GenerateNextHallway();});
            EventManager.OnRestart.RemoveListener(ResetGenID);
        }

        private void OnDestroy() {
            if (killed == false) {
                ResetGenID();
            }
        }

        private void ResetGenID() {
            nextGenID = 0;
            GameDB gameDB = GameDB.instance;
            gameDB.currentGenID = 0;
        }

        void Awake() {
            // set generator ID
            this.genID = nextGenID;
            // increment for next generator ID
            nextGenID++;
            //Debug.Log("AWAKE: GenerateLevel " + genID + " created at: " + x + "," + y + "," + z);
        }

        // Start is called before the first frame update
        void Start() {
            // set seed
            GameDB gameDB = GameDB.instance;
            gameDB.gameSeed = (gameDB.textSeed == 0 ? Random.Range(-2000000000, 2000000000) : gameDB.textSeed);
            int seed = gameDB.gameSeed;
            r = new System.Random(seed);
            // want initial path to branch a bit, others shouldn't branch as much to prevent lag
            isSpawningInitial = (this.genID == 0 ? true : false);
            maxSimultaneousTurns = (this.genID == 0 ? 2 : 1);
            // initial hallway creation
            //initialHallway = new List<SegVals>();
            // create spawn segment in case this is the first generator
            if (this.genID == 0) {
                // for first hallway, parent is itself
                this.parentGenID = 0;
                generatorsList = new List<GameObject>();
                // set hallway list variables
                hallwayList = new List<List<SegVals>>();
                // spawn segment initialization
                SegVals segvals = InstantiateSegment(-1);
                MoveToNewPos(segvals.segmentScript);
                initialHallway.Add(segvals);
                hallwayList.Add(initialHallway);
                rotation = 0;
                // place player on perch
                player = GameObject.Find("Player Hitbox");
                player.transform.position = new Vector3(0, 15, 0);
                // generate first hallways for player
                for (int i = 0; i < 2; i++) {
                    GenerateNextHallway();
                }
            }
            // for all other generators: just add the fork piece to the front of the hallwaylist
            else {
                hallwayList.Add(initialHallway);
            }
            // store this generator in the global list of GameObjects
            generatorsList.Add(gameObject);
            Debug.Log("START: GenerateLevel " + genID + " created at: " + x + "," + y + "," + z);
        }

        // Update is called once per frame
        void Update() {
            //if (isSpawningInitial) {
                // Spawn player and initial segments
            //    InitialSpawn();
            //}
        }

        private void InitialSpawn() {
            if ((numTurns < maxSimultaneousTurns)) {
                // pick a random segment prefab, excluding the turns if one has been placed recently
                int startRand = (lastTurn > turnThreshold ? 0 : 6);
                int randomInd = r.Next(startRand, segmentPrefabs.Count);
                if (randomInd < 6) {
                    // don't want branching paths in initial spawn
                    randomInd = randomInd % 2;
                    lastTurn = 0;
                    numTurns++;
                }
                // pick based on probability
                SegVals segvals = InstantiateSegment(randomInd);
                // rotate for a turn
                RotateForTurn(randomInd);
                // move spawner to new position
                MoveToNewPos(segvals.segmentScript);
                // add the segment to the hallway
                initialHallway.Add(segvals);
                // add to generation variables
                lastTurn++;
            }
            else {
                // done spawning initial hallway
                isSpawningInitial = false;
                hallwayList.Add(initialHallway);
            }
        }

        private void GenerateNextHallway() {
            Debug.Log("Generator " + genID + " trying to generate a hallway. Has " + hallwayList.Count + " hallways.");
            GameDB gameDB = GameDB.instance;
            // for turns that don't trigger spawn segments
            if (gameDB.currentGenID == -1) {
                return;
            }
            // don't generate a segment if this isn't the current generator or a future hallway
            if (killed == true || (genID != gameDB.currentGenID && hallwayList.Count > 1)) {
                //Debug.Log("Generator " + genID + " failed to generate a hallway. Has " + hallwayList.Count + " hallways.");
                return;
            }
            // Generate a number between 2 and 9 for the number of segments
            int numNewSegments = r.Next(2, 10); // the number of segments in this hallway
            //Debug.Log("Generator " + genID + " wants to generate a hallway with " + numNewSegments + " segments in it.");
            // Generate those segments
            List<SegVals> hallway = new List<SegVals>();

            if (hallwayList.Count > 3) {
                Debug.Log("Generator " + genID + " is trying to destroy a hallway. Has " + hallwayList.Count + " hallways.");
                DestroyHallway(hallwayList[0]);
                hallwayList.Remove(hallwayList[0]);
            }
            if (genID == gameDB.currentGenID) {
                Debug.Log("Generator " + genID + " is trying to kill the generators. " + hallwayList.Count + " hallways.");
                KillGenerator(genID);
            }

            for (int s = 0; s < numNewSegments; s++) {
                //Debug.Log("Generator " + genID + " is creating a new segment (" + s + "). Hallways: " + hallwayList.Count);
                int randomInd = r.Next(6, segmentPrefabs.Count); // intermediate segments are never turns
                SegVals segvals = InstantiateSegment(randomInd);
                MoveToNewPos(segvals.segmentScript);
                hallway.Add(segvals);
            }
            
            // Generate a turn
            int turnInd = r.Next(0, (canFork == true ? 6 : 2)); // left or right

            SegVals segvalsturn = InstantiateSegment(turnInd);

            RotateForTurn(turnInd);

            MoveToNewPos(segvalsturn.segmentScript);
            hallway.Add(segvalsturn);

            hallwayList.Add(hallway);
            canFork = (turnInd < 2 ? true : false);
            //Debug.Log("Generator " + genID + " succeeded to generate a hallway. Has " + hallwayList.Count + " hallways.");
        }

        private void RotateForTurn(int randomInd) {
            // deal with turns by resetting the angle and rotating in the correct direction
            // want to rotate 270 unity degrees if turning left, 90 if turning right
            switch (randomInd) {
                case 0:
                case 2:
                case 4:
                    rotation = rotation + 270;
                    break;
                case 1:
                case 3:
                case 5:
                    rotation = rotation + 90;
                    break;
            }
        }

        private void MoveToNewPos(SegmentScript segmentScript) {
            // set new x, y, z positions
            x = x + ((rotation % 180 == 90 ? segmentScript.length : 0) * (rotation % 360 == 90 ? 1 : -1));
            y = y + segmentScript.deltay;
            z = z + ((rotation % 180 == 0 ? segmentScript.length : 0) * (rotation % 360 == 0 ? 1 : -1));
            // move level generator to new position
            transform.position = new Vector3(x, y, z);
        }

        private SegVals InstantiateSegment(int segmentInd) {
            // pick segment based on probability
            SegVals segvals = new SegVals();
            // special case: spawn segment
            if (segmentInd == -1) {
                segvals.segment = Instantiate((GameObject)spawnSegment, new Vector3(x, y, z), Quaternion.Euler(new Vector3(0, rotation, 0)));
                segvals.segmentScript = segvals.segment.gameObject.GetComponent<SegmentScript>();
                segvals.segmentScript.segmentID = numSegments;
                segvals.segmentScript.generatorID = genID;
                segvals.segmentGenID = genID;
                segvals.numHallways = 1;
            }
            // normal case: spawn a chosen segment
            else {
                segvals.segment = Instantiate(segmentPrefabs[segmentInd], new Vector3(x, y, z), Quaternion.Euler(new Vector3(0, rotation, 0)));
                segvals.segmentScript = segvals.segment.gameObject.GetComponent<SegmentScript>();
                segvals.segmentScript.segmentID = numSegments;
                segvals.segmentScript.generatorID = genID;
                segvals.segmentGenID = genID;
                segvals.numHallways = 1;
                if (segmentInd == 0 || segmentInd == 1) {
                    Transform turn = segvals.segment.transform.Find("CornerPivot");
                    CornerTurnPlayer turnCTP = turn.GetComponent<CornerTurnPlayer>();
                    turnCTP.generator = genID;
                }
                // fork segments
                // the basic premise: we want to make another generator at the current position, giving it all the variables
                // that the current generator has
                else if (segmentInd == 2 || segmentInd == 3) {
                    Vector3 generateLevelPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
                    GameObject generateClone = Instantiate(gameObject, generateLevelPos, Quaternion.Euler(new Vector3(0, rotation, 0)));
                    GenerateLevel generateLevelClone = generateClone.GetComponent<GenerateLevel>();
                    // TODO: move this to object constructor of some sort?
                    generateLevelClone.x = x;
                    generateLevelClone.y = y;
                    generateLevelClone.z = z;
                    // temporary; will get updated on start() call
                    generateLevelClone.parentGenID = genID;
                    generateLevelClone.hallwayList = new List<List<SegVals>>();//(hallwayList);
                    generateLevelClone.initialHallway.Add(segvals);
                    generateLevelClone.rotation = rotation;
                    generateLevelClone.MoveToNewPos(segvals.segmentScript);

                    // add fork to multiple hallways to prevent immediate deletion
                    segvals.numHallways = 2;

                    Transform turn = segvals.segment.transform.Find("CornerPivot1");
                    CornerTurnPlayer turnCTP = turn.GetComponent<CornerTurnPlayer>();
                    turnCTP.generator = genID;

                    Transform fwd = segvals.segment.transform.Find("CornerPivot2");
                    CornerTurnPlayer fwdCTP = fwd.GetComponent<CornerTurnPlayer>();
                    fwdCTP.generator = generateLevelClone.genID;
                }
                // left right fork: player has to turn/choose a path
                else if (segmentInd == 4 || segmentInd == 5) {
                    Vector3 generateLevelPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z);
                    GameObject generateClone = Instantiate(gameObject, generateLevelPos, Quaternion.Euler(new Vector3(0, rotation, 0)));
                    GenerateLevel generateLevelClone = generateClone.GetComponent<GenerateLevel>();
                    // TODO: move this to object constructor of some sort?
                    generateLevelClone.x = x;
                    generateLevelClone.y = y;
                    generateLevelClone.z = z;
                    // temporary; will get updated on start() call
                    generateLevelClone.parentGenID = genID;
                    generateLevelClone.hallwayList = new List<List<SegVals>>();//(hallwayList);
                    generateLevelClone.initialHallway.Add(segvals);
                    generateLevelClone.rotation = rotation + (segmentInd == 4 ? 90 : 270);
                    generateLevelClone.MoveToNewPos(segvals.segmentScript);

                    // add fork to multiple hallways to prevent immediate deletion
                    segvals.numHallways = 2;

                    Transform turn = segvals.segment.transform.Find("CornerPivot1");
                    CornerTurnPlayer turnCTP = turn.GetComponent<CornerTurnPlayer>();
                    turnCTP.generator = genID;

                    Transform turn2 = segvals.segment.transform.Find("CornerPivot2");
                    CornerTurnPlayer turn2CTP = turn2.GetComponent<CornerTurnPlayer>();
                    turn2CTP.generator = generateLevelClone.genID;
                }
            }
            numSegments++;
            return segvals;
        }

        // main idea: when a generator from a fork is no longer being used, kill it, remove it from the
        // list of generators, and then destroy it so that no more segments can spawn from it.
        // setting the killed flag prevents genID from resetting to 0.
        private void KillGenerator(int segGenID) {
            Debug.Log(numSegments + " KillGenerator has been called by " + segGenID);
            int numkilled = 0;
            foreach (GameObject generator in generatorsList) {
                GenerateLevel genscript = generator.GetComponent<GenerateLevel>();
                // DON'T delete this generator or any ones that haven't been branched yet
                if (genscript.genID == segGenID || genscript.hallwayList.Count <= 1) {
                    continue;
                }
                // if you didn't visit the segment, kill it.
                genscript.killed = true;
                numkilled++;
                Debug.Log(numSegments + " Generator " + genscript.genID + " was killed.");
                // delete the segments from this generator
                while (genscript.hallwayList.Count > 0) {
                    List<SegVals> segmentsToDelete = genscript.hallwayList[0];
                    DestroyHallway(segmentsToDelete);
                    genscript.hallwayList.Remove(segmentsToDelete);
                }
            }
            // destroy the generators themselves
            for (int i = 0; i < generatorsList.Count; i++) {
                GameObject generator = generatorsList[i];
                GenerateLevel genscript = generator.GetComponent<GenerateLevel>();
                if (genscript.killed == true) {
                    generatorsList.Remove(generator);
                    Destroy(generator);
                }
            }
            //Debug.Log(numSegments + " " + numkilled + " Generator killed.");
        }

        private void DestroyHallway(List<SegVals> hallway) {
            foreach (SegVals segvals in hallway){
                GameObject segment = segvals.segment;
                // watnt to delete the segment if it is only part of the hallway being deleted
                if (segvals.numHallways == 1) {
                    Destroy(segment);
                }
                // if another hallway has this segment, just decrement how many hallways the segment is in
                else {
                    segvals.numHallways -= 1;
                }
            }
        }
    }
}