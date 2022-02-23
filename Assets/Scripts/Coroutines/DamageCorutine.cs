using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCorutine : MonoBehaviour
{
    private TextMesh textMesh;
    [SerializeField] CharacterComponent characterComponent;

    private void Start()
    {
        textMesh = GetComponent<TextMesh>();   
    }
    public IEnumerator Flashing()
    {   
         
        for(float i = 0; i < 1; i+= Time.deltaTime)
        {
            textMesh.color = Color.Lerp(textMesh.color, Color.red, i);
            yield return null;
        }
        yield return new WaitForSeconds(0.1f);
        for (float i = 0; i < 1; i += Time.deltaTime)
        {
            textMesh.color = Color.Lerp(textMesh.color, Color.white, i);
            yield return null;
        }
       
    }
}
