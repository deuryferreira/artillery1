using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>Construye el Canvas y conecta sus controles para que la escena sea autónoma.</summary>
public sealed class ConstruirInterfaz : MonoBehaviour
{
    private void Awake()
    {
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject sistema = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            sistema.transform.SetParent(transform, false);
        }
        InterfazJuego control = GetComponent<InterfazJuego>();
        Canvas canvas = new GameObject("Canvas", typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster)).GetComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        CanvasScaler scaler = canvas.GetComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);

        Text estado = CrearTexto(canvas.transform, "Disparos: 0/5", new Vector2(0, 0.92f), new Vector2(0.3f, 0.98f), 26);
        Slider slider = CrearSlider(canvas.transform);
        Text valor = CrearTexto(canvas.transform, "Fuerza: 18", new Vector2(0, 0.86f), new Vector2(0.3f, 0.92f), 22);
        slider.onValueChanged.AddListener(v => valor.text = "Fuerza: " + Mathf.RoundToInt(v));
        control.Configurar(slider, CrearMenu(canvas.transform, control), estado);
    }

    private static GameObject CrearMenu(Transform root, InterfazJuego control)
    {
        GameObject panel = new GameObject("Menu", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(root, false);
        RectTransform r = panel.GetComponent<RectTransform>(); r.anchorMin = new Vector2(.5f,.5f); r.anchorMax=r.anchorMin; r.sizeDelta=new Vector2(420,350);
        panel.GetComponent<Image>().color = new Color(.06f,.09f,.15f,.94f);
        Text titulo = CrearTexto(panel.transform,"ARTILLERY",new Vector2(.05f,.76f),new Vector2(.95f,.96f),34);
        Button jugar = CrearBoton(panel.transform,"Jugar",new Vector2(.15f,.50f),new Vector2(.85f,.68f)); jugar.onClick.AddListener(control.Jugar);
        Button reiniciar = CrearBoton(panel.transform,"Volver a jugar",new Vector2(.15f,.29f),new Vector2(.85f,.47f)); reiniciar.onClick.AddListener(control.Reiniciar);
        Button salir = CrearBoton(panel.transform,"Salir",new Vector2(.15f,.08f),new Vector2(.85f,.26f)); salir.onClick.AddListener(control.Salir);
        control.ConfigurarTitulo(titulo);
        return panel;
    }

    private static Slider CrearSlider(Transform root)
    {
        GameObject go=new GameObject("SliderFuerza",typeof(RectTransform),typeof(Slider)); go.transform.SetParent(root,false);
        RectTransform rect=go.GetComponent<RectTransform>(); rect.anchorMin=new Vector2(.04f,.86f); rect.anchorMax=new Vector2(.28f,.90f); rect.offsetMin=rect.offsetMax=Vector2.zero;
        Image fondo=go.AddComponent<Image>(); fondo.color=new Color(.18f,.2f,.24f);
        GameObject fill=new GameObject("Fill",typeof(RectTransform),typeof(Image)); fill.transform.SetParent(go.transform,false);
        RectTransform fr=fill.GetComponent<RectTransform>(); fr.anchorMin=new Vector2(0,.25f); fr.anchorMax=new Vector2(1,.75f); fr.offsetMin=Vector2.zero; fr.offsetMax=Vector2.zero;
        Image fi=fill.GetComponent<Image>(); fi.color=new Color(.2f,.65f,.95f);
        GameObject handle=new GameObject("Handle",typeof(RectTransform),typeof(Image)); handle.transform.SetParent(go.transform,false);
        Image hi=handle.GetComponent<Image>(); hi.color=Color.white;
        RectTransform hr=handle.GetComponent<RectTransform>(); hr.sizeDelta=new Vector2(24,32);
        Slider s=go.GetComponent<Slider>(); s.fillRect=fr; s.handleRect=hr; s.targetGraphic=hi; s.minValue=5; s.maxValue=40; s.value=18;
        return s;
    }

    private static Text CrearTexto(Transform root,string contenido,Vector2 min,Vector2 max,int size)
    {
        GameObject go=new GameObject("Texto",typeof(RectTransform),typeof(Text)); go.transform.SetParent(root,false);
        RectTransform r=go.GetComponent<RectTransform>(); r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;
        Text t=go.GetComponent<Text>();t.text=contenido;t.font=Resources.GetBuiltinResource<Font>("Arial.ttf");t.fontSize=size;t.alignment=TextAnchor.MiddleCenter;t.color=Color.white;return t;
    }
    private static Button CrearBoton(Transform root,string label,Vector2 min,Vector2 max)
    {
        GameObject go=new GameObject(label,typeof(RectTransform),typeof(Image),typeof(Button));go.transform.SetParent(root,false);
        RectTransform r=go.GetComponent<RectTransform>();r.anchorMin=min;r.anchorMax=max;r.offsetMin=r.offsetMax=Vector2.zero;
        go.GetComponent<Image>().color=new Color(.12f,.42f,.72f);CrearTexto(go.transform,label,Vector2.zero,Vector2.one,22);return go.GetComponent<Button>();
    }
}
