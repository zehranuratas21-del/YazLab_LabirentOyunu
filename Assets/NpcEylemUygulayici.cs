using UnityEngine;

// LLM'in seçtiği eylemi oyun dünyasında uygulayan sınıfların tabanı.
// Yeni bir eylem türü için bunu miras alan yeni bir sınıf yazılır.
public abstract class NpcEylemUygulayici : MonoBehaviour
{
    public abstract void Uygula(string eylem);
}
