using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Scripts
{
    public class GridSquare : MonoBehaviour, IPointerClickHandler
    {
        //this game object has the tag of "GridSquare"
    
        //is the player or npc on top of this grid square
        public bool current = false;

        //did the player or character select this grid square to move to
        public bool target;
    
        //is this grid square selectable by the player
        public bool selectable;
    
        //is the player allowed to walk on this grid square
        public bool walkable = true;

        //variable for BFS
        public GridSquare parent = null;
        
        public int xCoordinate, zCoordinate;

        public bool APathTest;

        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            //https://docs.unity3d.com/ScriptReference/Material.SetColor.html
            if (current)
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.red);
            } else if (target)
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.blue);
            } else if (selectable)
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
            } else if (APathTest)
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.magenta);
            }
            else
            {
                gameObject.GetComponent<MeshRenderer>().material.SetColor("_Color", new Color(0.2169f, 0.8679f, 0.6573f));
            }
            
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            //Debug.Log("BANANAS!!!! ");

        }

        //reset all variables after a turn is played
        public void Reset()
        {
            target = false;
            selectable = false;
            parent = null;
        }
    }
}
