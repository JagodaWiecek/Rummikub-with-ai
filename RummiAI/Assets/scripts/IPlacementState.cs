using UnityEngine;
/// <summary>
/// Interfejs do poruszania, usuwania i tworzenia 3d płytek na mapie
/// </summary>
public interface IPlacementState
{
    /// <summary>
    /// Funkcja wykonująca się na końcu poruszania obiektami
    /// </summary>
    void EndState();

    /// <summary>
    /// Funkcja, wykonująca się po kliknięciu prawym przyciskiem myszy
    /// </summary>
    /// <param name="gridPosition">pobierana pozycja obiektu grid na mapie</param>
    void OnAction(Vector3Int gridPosition);
    /// <summary>
    /// funkcja zmieniająca pozycję obiektu grid na bierząco w zależności od pozycji kursowa myszy
    /// </summary>
    /// <param name="gridPosition">pozycja grida na mapie</param>
    void UpdateState(Vector3Int gridPosition);

    /// <summary>
    /// funkcja do uzyskania pozycji grida w 
    /// </summary>
    /// <returns>pozycję grida na mapie</returns>
    Vector3Int GetGridPosition();


}