using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour 
{
    private void OnPreCull () 
    {
        List<Portal> portals = Portal.allPortals;
        for (int i = 0; i < portals.Count; i++) {
            portals[i].PrePortalRender ();
        }
        for (int i = 0; i < portals.Count; i++) {
            portals[i].Render ();
        }

        for (int i = 0; i < portals.Count; i++) {
            portals[i].PostPortalRender ();
        }

    }
}