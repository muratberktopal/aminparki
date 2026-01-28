using UnityEngine;

public class ModelGenerator : MonoBehaviour
{
    public Color coatColor = Color.black;
    public Color skinColor = new Color(1f, 0.8f, 0.6f);
    public Color hatColor = new Color(0.2f, 0.2f, 0.2f);

    [ContextMenu("Karakteri Ýnþa Et")]
    public void BuildCharacter()
    {
        // Temizlik
        while (transform.childCount > 0) DestroyImmediate(transform.GetChild(0).gameObject);

        // --- HATA DÜZELTÝCÝ: Shader Bulma ---
        // Önce URP shader'ý ara, bulamazsan Eskisini (Standard) dene
        Shader shader = Shader.Find("Universal Render Pipeline/Lit");
        if (shader == null) shader = Shader.Find("Standard");

        // Eðer hala bulamazsa (Çok nadir), Unity'nin default shader'ýný ver
        if (shader == null) shader = Shader.Find("Diffuse");

        Material coatMat = new Material(shader); coatMat.color = coatColor;
        Material skinMat = new Material(shader); skinMat.color = skinColor;
        Material hatMat = new Material(shader); hatMat.color = hatColor;

        // --- GÖVDE ---
        CreatePart(PrimitiveType.Cylinder, "Body_Coat", new Vector3(0, 1f, 0), new Vector3(0.8f, 1f, 0.8f), coatMat, transform);
        // --- KAFA ---
        GameObject head = CreatePart(PrimitiveType.Sphere, "Head", new Vector3(0, 2.1f, 0), Vector3.one * 0.5f, skinMat, transform);
        // --- ÞAPKA ---
        CreatePart(PrimitiveType.Cylinder, "Hat_Brim", new Vector3(0, 0.35f, 0), new Vector3(1.5f, 0.1f, 1.5f), hatMat, head.transform);
        CreatePart(PrimitiveType.Cylinder, "Hat_Top", new Vector3(0, 0.6f, 0), new Vector3(0.8f, 0.5f, 0.8f), hatMat, head.transform);
        // --- KOLLAR ---
        CreatePart(PrimitiveType.Cube, "RightArm_Holder", new Vector3(0.6f, 1.7f, 0), new Vector3(0.25f, 0.9f, 0.25f), coatMat, transform);
        CreatePart(PrimitiveType.Cube, "LeftArm_Holder", new Vector3(-0.6f, 1.7f, 0), new Vector3(0.25f, 0.9f, 0.25f), coatMat, transform);
        // --- YAKA ---
        CreatePart(PrimitiveType.Cube, "Collar", new Vector3(0, 1.9f, -0.15f), new Vector3(0.9f, 0.3f, 0.5f), coatMat, transform);

        Debug.Log("Morluk geçti mi hacým?");
    }

    // Kod tekrarýný önlemek için yardýmcý fonksiyon
    GameObject CreatePart(PrimitiveType type, string name, Vector3 pos, Vector3 scale, Material mat, Transform parent)
    {
        GameObject obj = GameObject.CreatePrimitive(type);
        obj.name = name;
        obj.transform.parent = parent;
        obj.transform.localPosition = pos;
        obj.transform.localScale = scale;
        obj.GetComponent<Renderer>().material = mat;
        DestroyImmediate(obj.GetComponent<Collider>()); // Collider çakýþmasýn
        return obj;
    }
}