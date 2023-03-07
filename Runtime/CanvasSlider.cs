using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasSlider : MonoBehaviour
{
    [SerializeField] private GameObject _FakeFinger; // fake finger collider to keep button pressed down while sliding

    private bool stopDisable;

    // Makes slider button pressed when finger is taken from button, but slider is still being dragged
    public void SliderBeginDrag()
    {
        StopAllCoroutines(); // prop not needed
        stopDisable = true;
        _FakeFinger.SetActive(true);
        _FakeFinger.transform.localPosition = Vector3.zero;
    }

    // 
    public void SliderEndDrag()
    {
        stopDisable = false;
        _FakeFinger.transform.localPosition = new Vector3(0, 0, 50);
    }

    IEnumerator SmallDelay ()
    {
        yield return new WaitForSeconds(0.4f);
        if (!stopDisable) _FakeFinger.SetActive(false);
    }
}
