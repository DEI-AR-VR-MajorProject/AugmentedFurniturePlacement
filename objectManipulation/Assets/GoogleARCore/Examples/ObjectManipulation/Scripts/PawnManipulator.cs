//-----------------------------------------------------------------------
// <copyright file="PawnManipulator.cs" company="Google">
//
// Copyright 2019 Google LLC. All Rights Reserved.
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
//
// </copyright>
//-----------------------------------------------------------------------

namespace GoogleARCore.Examples.ObjectManipulation
{
    using GoogleARCore;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;
    using GoogleARCore.Examples.ObjectManipulationInternal;
    

    /// <summary>
    /// Controls the placement of objects via a tap gesture.
    /// </summary>
    public class PawnManipulator : Manipulator
    {
        /// <summary>
        /// The first-person camera being used to render the passthrough camera image (i.e. AR
        /// background).
        /// </summary>
        public Camera FirstPersonCamera;

        /// <summary>
        /// A prefab to place when a raycast from a user touch hits a plane.
        /// </summary>
       
        public GameObject SelectedPrefab;

        /// <summary>
        /// Manipulator prefab to attach placed objects to.
        /// </summary>
        public GameObject ManipulatorPrefab;

        /// <summary>
        /// Returns true if the manipulation can be started for the given gesture.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        /// <returns>True if the manipulation can be started.</returns>
        /// 


        public RawImage Img;
        public GameObject ButtonPrefab;
        public GameObject DeleteButton;
        public Transform content;
        public Dropdown dropdown;
        public Dropdown DropdownColour;
        int FlagTag = 0;

        List<string> options = new List<string>() { "Selection Menu", "Table", "Chair", "Bench", "Stool", "Miscellaneous" };
        List<string> ColourOptions = new List<string>() { "Select colour", "Red", "Blue", "Black", "Yellow", "White","Default"};

        public Material DefaultMaterial;// SetMaterial;
        Color colour;
       // public Material Red,Blue,Black,Yellow,White;

        void Start()
        {
           // PopulateButtons();
            PopulateDropdown();
            PopulateDropdownColour();
           
        }


        public void Dropdown_SelectedIndex(int index)
        {
            if (index == 0) { } // nothing selected => do nothing
            if (index == 1) { PopulateButtons("Table"); } // populate
            if (index == 2) { PopulateButtons("Chair"); }
            if (index == 3) { PopulateButtons("Bench"); }
            if (index == 4) { PopulateButtons("Stool"); }
            if (index == 5) { PopulateButtons("Miscellaneous"); }
            
        }

        public void DropdownColour_SelectedIndex(int index)
        {
            if (index == 0) { } // nothing selected => do nothing
            if (index == 1) { colour = Color.red; }
            if (index == 2) { colour = Color.blue; }
            if (index == 3) { colour = Color.black; }
            if (index == 4) { colour = Color.yellow; }
            if (index == 5) { colour = Color.white; }
            if (index == 6) { colour = DefaultMaterial.color; }

            FindFurniturePrefab("ChangeColour",colour); 
        }

        private void PopulateDropdown()
        {
            dropdown.AddOptions(options);
        }

        private void PopulateDropdownColour()
        {
            DropdownColour.AddOptions(ColourOptions);
        }


        private void PopulateButtons( string selectedOption)
        {

            foreach (Transform child in content.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            Texture[] textures = Resources.LoadAll<Texture>(selectedOption);

            foreach (Texture t in textures)
            {
                //Debug.Log(t);
                if (t.name.Equals("error") || t.name.Equals("download") || t.name.Equals("loading"))
                    Debug.Log(t);
                else
                {
                    GameObject ThisButton = Instantiate(ButtonPrefab) as GameObject;

                    //button resizing .....this section is not working!
                    RectTransform rT = ThisButton.GetComponent<RectTransform>();
                    rT.sizeDelta = new Vector2(rT.sizeDelta.x, (Screen.height/3));


                    ThisButton.transform.SetParent(content);
                    ThisButton.GetComponent<RawImage>().texture = t;

                    ThisButton.GetComponentInChildren<Text>().text = t.name;
                    ThisButton.name = t.name;

                    ThisButton.GetComponent<Button>().onClick.AddListener(() => OnClickSelectFurniture(t.name));

                }

                DeleteButton.GetComponent<Button>().onClick.AddListener(() => DeleteSelectedFurniture());
               // DeleteButton.GetComponent<Button>().onClick.AddListener(() => ButtonClicked());
            }
        }


          //changed***********************************************************************

            bool shiftOn = false;
            public void ButtonClicked()
            {
                shiftOn = !shiftOn;
                if (shiftOn)
                {
                    DeleteButton.GetComponent<Renderer>().material.color = Color.gray;
                    DeleteSelectedFurniture();
                }
                else
                    DeleteButton.GetComponent<Renderer>().material.color = Color.red;
            }        


            public void DeleteSelectedFurniture()
            {
                
               // FindFurniturePrefab("Delete",Color.magenta); // passing any color, it won't be used anyhow           
                

                Destroy(GameObject.Find(SelectedPrefab.name));
            }


            public LayerMask touchableLayers;

            private void FindFurniturePrefab(string task, Color MaterialColour)
            {
                SelectedPrefab.GetComponent<Renderer>().material.color = MaterialColour;//********************************
                var camera = Camera.main;
                string TargetObject;

                int count = Input.touchCount;
                for (int i = 0; i < count; i++)
                {
                    var touch = Input.GetTouch(i);

                    if (touch.position.x < 7 * Screen.width / 9)
                    {

                        var ray = camera.ScreenPointToRay(touch.position);

                        RaycastHit hit;
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, touchableLayers))
                        {
                            TargetObject = hit.collider.gameObject.tag;
                            if (task == "Delete")
                            {
                                Debug.LogFormat("Object {0} got touched!", hit.collider.gameObject.name);
                                //TargetObject = hit.collider.gameObject.tag;
                                Destroy(GameObject.FindGameObjectWithTag(TargetObject));
                            }
                            else if (task=="ChangeColour")
                            {
                                GameObject.FindGameObjectWithTag(TargetObject).GetComponent<Renderer>().material.color = MaterialColour;
                                SelectedPrefab.GetComponent<Renderer>().material.color = MaterialColour;
                            }
                        }
                    }
                }
            }


             


            public void OnClickSelectFurniture(string name)
            {

                GameObject[] AllPrefabs = Resources.LoadAll<GameObject>("FurniturePrefabs");

               foreach (GameObject p in AllPrefabs)
               {
                   if (p.name.Equals(name))
                   {
                       Debug.Log(p.name);
                       SelectedPrefab = p;
                       DefaultMaterial = SelectedPrefab.GetComponent<Renderer>().material;
                       Debug.Log(SelectedPrefab.name);
                   }
               }
             
            }
       
    

        protected override bool CanStartManipulationForGesture(TapGesture gesture)
        {
            if (gesture.TargetObject == null)
            {
                return true;
            }

            return false;
        }

        /// <summary>
        /// Function called when the manipulation is ended.
        /// </summary>
        /// <param name="gesture">The current gesture.</param>
        protected override void OnEndManipulation(TapGesture gesture)
        {
            if (gesture.WasCancelled)
            {
                return;
            }

            // If gesture is targeting an existing object we are done.
            if (gesture.TargetObject != null)
            {
                return;
            }

            // Raycast against the location the player touched to search for planes.
            TrackableHit hit;
            TrackableHitFlags raycastFilter = TrackableHitFlags.PlaneWithinPolygon;

            if (Frame.Raycast(
                gesture.StartPosition.x, gesture.StartPosition.y, raycastFilter, out hit))
            {
                // Use hit pose and camera pose to check if hittest is from the
                // back of the plane, if it is, no need to create the anchor.
                if ((hit.Trackable is DetectedPlane) &&
                    Vector3.Dot(FirstPersonCamera.transform.position - hit.Pose.position,
                        hit.Pose.rotation * Vector3.up) < 0)
                {
                    Debug.Log("Hit at back of the current DetectedPlane");
                }
                else
                {

                     Touch[] myTouches = Input.touches;
                     for (int i = 0; i < Input.touchCount; i++)
                     {
                         Touch myTouch = Input.GetTouch(i);

                         //Set start postition
                        
                         
                             //Left Thumb Stick
                             if (myTouch.position.x < 7*Screen.width / 9)
                             {
                                 FlagTag++;
                                 // Instantiate game object at the hit pose.                                 
                                 var gameObject = Instantiate(SelectedPrefab, hit.Pose.position, hit.Pose.rotation);
                                 SelectedPrefab.tag = FlagTag.ToString(); // To keep track of placed furnitures for deleting if required**********
                                 //var gameObject = Instantiate(SelectedPrefab, hit.Pose.position, hit.Pose.rotation);

                                 // Instantiate manipulator.
                                 var manipulator =
                                     Instantiate(ManipulatorPrefab, hit.Pose.position, hit.Pose.rotation);

                                 // Make game object a child of the manipulator.
                                 gameObject.transform.parent = manipulator.transform;

                                 // Create an anchor to allow ARCore to track the hitpoint as understanding of
                                 // the physical world evolves.
                                 var anchor = hit.Trackable.CreateAnchor(hit.Pose);

                                 // Make manipulator a child of the anchor.
                                 manipulator.transform.parent = anchor.transform;

                                 // Select the placed object.
                                 manipulator.GetComponent<Manipulator>().Select();
                             }


                         
                     }
                    
                }
            }
        }
    }
}
