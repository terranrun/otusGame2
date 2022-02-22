using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class HPCorutiens : MonoBehaviour
{
   
    [SerializeField] private HealthComponent _healthComponent;
   

    
    private Image _image;

    private void Awake()
    {
        
        _image = GetComponent<Image>();
        _image.fillAmount = 1;
    }
    private void Start()
    {
        StartCoroutine(Filling());
    }
    private IEnumerator Filling( )
    {
        float startValue = 1;
            while (startValue>0)
            {
                startValue = _healthComponent.Health/10f;
                _image.fillAmount = startValue;
                yield return new WaitUntil(() => _healthComponent.IsDead == false);
            }   
    } 
}
