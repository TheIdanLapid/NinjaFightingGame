using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class StatBarUI : MonoBehaviour
{
    [SerializeField] float initValue = 1;
    [SerializeField] Image fill = default;
    // Start is called before the first frame update
    void Awake()
    {
        fill.fillAmount = initValue;
    }

    // Update is called once per frame
    public void UpdateBar(float fillValue)
    {
        fill.fillAmount = fillValue;
    }
}
