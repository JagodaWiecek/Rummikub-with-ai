using UnityEngine;
//Klasa odpowiedzialna za utworzenie obiektu wizualnego dla gracza.
//Stworzony obiekt przemieszcza siê po mapie wraz z kursorem u¿ytkownika, zmienia on
//kolor w zale¿noœci, czy interakcje mo¿na wykonaæ czy nie.
public class PreviewSystem : MonoBehaviour
{
    [SerializeField]
    private float previewYOffset = 0.06f;

    [SerializeField]
    private GameObject cellIndicator;
    private GameObject previewObject;

    [SerializeField]
    private Material previewMaterialPrefab; 
    private Material previewMaterialInstance;

    private Renderer cellIndicatorRenderer;

    /// <summary>
    /// funkcja rozpoczynaj¹ca siê przed pierwsz¹ klatk¹ 
    /// </summary>
    private void Start()
    {
        previewMaterialInstance = new Material(previewMaterialPrefab);
        cellIndicator.SetActive(false);
        cellIndicatorRenderer = cellIndicator.GetComponentInChildren<Renderer>();
    }
    /// <summary>
    /// funkcja tworz¹ca obiekt wizualny na mapie dla funkcji dodawania obiektu i przesuwania
    /// </summary>
    /// <param name="prefabe">otworzony obiekt</param>
    /// <param name="size">rozmiar obiektu</param>
    internal void StartShowingPlacementPreview(GameObject prefabe, Vector2Int size)
    {
        previewObject = Instantiate(prefabe);
        PreparePreview(previewObject);
        PrepareCursor(size);
        cellIndicator.SetActive(true);
    }
    /// <summary>
    /// funkcja do ustalenia wielkoœci kursora
    /// </summary>
    /// <param name="size"></param>
    private void PrepareCursor(Vector2Int size)
    {
        if (size.x > 0 || size.y > 0)
        {
            cellIndicator.transform.localScale = new Vector3(size.x,1,size.y);
            cellIndicatorRenderer.material.mainTextureScale = size;
        }
    }
    /// <summary>
    /// funkcja do przygotowania obiektu
    /// </summary>
    /// <param name="previewObject">prefab obiektu</param>
    private void PreparePreview(GameObject previewObject)
    {
        Renderer[] renderers = previewObject.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in renderers)
        {
            Material[] materials = renderer.materials;
            for(int i = 0; i < materials.Length; i++)
            {
                materials[i] = previewMaterialInstance;
            }
            renderer.materials = materials;
        }
    }
    /// <summary>
    /// usuniêcie obiektu wizualnego dla funkcji usuwania obiektów
    /// </summary>
    public void StopShowingPreview()
    {
        cellIndicator.SetActive(false);
        if(previewObject != null) 
            Destroy(previewObject);
    }
    /// <summary>
    /// funkcja s³u¿¹ca do aktualizacji interakcji z obiektem
    /// </summary>
    /// <param name="position">pozycja docelowa obiektu</param>
    /// <param name="validity">zmienna do zmiany koloru obiektu</param>
    public void UpdatePosition(Vector3 position, bool validity)
    {
        if(previewObject != null){
            MovePreview(position);
            ApplyFeedbackToPreview(validity);

        }
        MoveCursor(position);
        ApplyFeedbackToCursor(validity);
    }
    /// <summary>
    /// funkcja s³u¿¹ca do zmiany koloru obiektu w zale¿noœci od wprowadzanej zmiennej do funkcji
    /// jeœli zmienna wchodz¹ca bêdzie true to kolor obiektu bêdzie bia³y
    /// jeœli zmienna bêdzie przecz¹ca, to zostanie przypisany kolor czerwony
    /// </summary>
    /// <param name="validity">zmienna boolean wprowadzana do funkcji</param>
    private void ApplyFeedbackToPreview(bool validity)
    {
        Color c = validity ? Color.white: Color.red;
        c.a = 0.5f;
        previewMaterialInstance.color = c;
    }
    /// <summary>
    /// funkcja zmienia kolor obiektu w zale¿noœci od wartoœci wprowadzanej do funkcji
    /// jeœli zmienna wchodz¹ca bêdzie true to kolor obiektu bêdzie bia³y
    /// jeœli zmienna bêdzie przecz¹ca, to zostanie przypisany kolor czerwony
    /// </summary>
    /// <param name="validity">zmienna boolean wykazuj¹ca poprawnoœæ</param>
    private void ApplyFeedbackToCursor(bool validity)
    {
        Color c = validity ? Color.white : Color.red;
        c.a = 0.5f;
        cellIndicatorRenderer.material.color = c;
    }
    /// <summary>
    /// zmiana pozycji obiektu wizualnego
    /// dla funkcji dodawania obiektu i przesuwania
    /// </summary>
    /// <param name="position"></param>
    private void MoveCursor(Vector3 position)
    {
        cellIndicator.transform.position = position;
    }
    /// <summary>
    /// zmiana pozycji obiektu wizualnego do usuwania obiektu na bazie podanej lokalizacji
    /// </summary>
    /// <param name="position">docelowa lokalizacja obiektu</param>
    private void MovePreview(Vector3 position)
    {
        previewObject.transform.position = new Vector3(position.x, position.y + previewYOffset,position.z);
    }
    /// <summary>
    /// funkcja do uruchomienia wizualnego obiektu dla funkcji usuwania obiektów
    /// na mapie
    /// </summary>
    internal void StartShowingRemovePreview()
    {
        cellIndicator.SetActive(true);
        PrepareCursor(Vector2Int.one);
        ApplyFeedbackToCursor(false);
    }
}
