using UnityEngine;
using System.Collections;
using SlimNet.Unity;

public class SlimNetActiveController : SlimNetMonoBehaviour
{
    bool lastState = true;

    void Update()
    {
        if (lastState != !networkActor.IsIdle)
        {
            lastState = !networkActor.IsIdle;

            for (int i = 0; i < transform.childCount; ++i)
            {
                transform.GetChild(i).gameObject.SetActiveRecursively(lastState);
            }
        }
    }
}
