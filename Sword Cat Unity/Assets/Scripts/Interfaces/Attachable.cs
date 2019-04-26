using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//having this interface means that the object can have Swords attached to it.
public interface Attachable
{
    //method for allowing a sword to attach to this object
    void attach(GameObject sword);
}
