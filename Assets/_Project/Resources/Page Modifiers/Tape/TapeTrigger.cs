using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class TapeTrigger : MonoBehaviour
{
    [Tooltip("The required speed difference between frames to trigger a 'too fast' event. \nHigher values make it easier, but it scales exponentially.")]
    [SerializeField] float requiredSpeed = 0.1f;
    [Tooltip("Whether the fill amount of the gauge is directly proportional to the trigger magnitude, or if it snaps to increments based on the values list." + 
             "\nThis setting is entirely cosmetic.")]
    [SerializeField] bool proportionalFill = false;
    
    readonly List<float> values = new ();

    [SerializeField] GameObject verticalRip;
    [SerializeField] GameObject horizontalRip;

    bool horizontal;
    Image gauge;
    TweenerCore<float, float, FloatOptions> tween;

    void Start()
    {
        horizontal = GetComponent<RectTransform>().rotation != Quaternion.identity;

        gauge = transform.GetChild(0).GetComponent<Image>();
        gauge.fillAmount = 0;
    }

    void Update()
    {
        if (Gamepad.current != null)
        {
            if (Gamepad.current.rightTrigger.IsActuated())
            {
                //Debug.Log(Gamepad.current.rightTrigger.magnitude);
                StartCoroutine(RipTape());
            }

            if (Gamepad.current.rightTrigger.wasReleasedThisFrame)
            {
                values.Clear();
            }
        }
    }
    
    IEnumerator RipTape()
    {
        float magnitudeThisFrame = Gamepad.current.rightTrigger.magnitude;
        yield return null;
        float magnitudeNextFrame = Gamepad.current.rightTrigger.magnitude;
        
        float delta = magnitudeNextFrame - magnitudeThisFrame;

        if (delta > requiredSpeed)
        {
            Debug.LogWarning("Too fast!");

            GameObject ripType = horizontal ? horizontalRip : verticalRip;
            var rip = Instantiate(ripType, transform.position, Quaternion.identity);
            rip.transform.SetParent(transform.parent, true);
            Destroy(gameObject);
        }
        else
        {
            values.Add(Mathf.Round(Gamepad.current.rightTrigger.magnitude * 10f) / 10f);
            Debug.Log(values.Last());

            if (tween.IsActive())
            {
                tween.ChangeEndValue(values.Last(), true);
            }
            else
            {
                if (proportionalFill) 
                    gauge.fillAmount = Gamepad.current.rightTrigger.magnitude; // directly proportional to trigger magnitude
                else
                    tween = gauge.DOFillAmount(values.Last(), 1.5f).SetEase(Ease.OutElastic).SetLink(gameObject); // snaps to increments of 0.1
            }
            
            if (values.Last() >= 1f) 
            {
                Destroy(gameObject);
                Debug.Log("Successfully removed!");
            }
        }
    }
}
