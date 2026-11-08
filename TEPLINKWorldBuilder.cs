using UnityEngine;

public class TEPLINKWorldBuilder : MonoBehaviour
{
    private Material road, sidewalk, wall, cream, red, blue, dark, green, glass, sand, sign;
    private void Start()
    {
        road = Mat(new Color(.16f, .17f, .18f));
        sidewalk = Mat(new Color(.52f, .48f, .41f));
        wall = Mat(new Color(.73f, .62f, .47f));
        cream = Mat(new Color(.88f, .79f, .64f));
        red = Mat(new Color(.78f, .12f, .10f));
        blue = Mat(new Color(.08f, .24f, .48f));
        dark = Mat(new Color(.08f, .09f, .10f));
        green = Mat(new Color(.14f, .35f, .20f));
        glass = Mat(new Color(.18f, .42f, .55f, .65f));
        sand = Mat(new Color(.57f, .47f, .34f));
        sign = Mat(new Color(.04f, .06f, .09f));

        BuildStreet();
        BuildBuildings();
        BuildCyberCafe();
        BuildHome();
        BuildServices();
        BuildDecor();
        BuildNPCs();
    }

    private Material Mat(Color c)
    {
        var m = new Material(Shader.Find("Standard"));
        m.color = c;
        return m;
    }

    private GameObject Cube(string name, Vector3 pos, Vector3 scale, Material mat, bool collider = true)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name; go.transform.SetParent(transform);
        go.transform.position = pos; go.transform.localScale = scale;
        if (mat != null) go.GetComponent<Renderer>().material = mat;
        if (!collider) { var c = go.GetComponent<Collider>(); if (c) Destroy(c); }
        return go;
    }

    private void Label(string text, Vector3 pos, float size = .28f, Color? color = null)
    {
        var go = new GameObject("Arabic sign: " + text);
        go.transform.SetParent(transform);
        go.transform.position = pos;
        go.transform.rotation = Quaternion.Euler(0, 0, 0);
        var tm = go.AddComponent<TextMesh>();
        tm.text = text; tm.fontSize = 64; tm.characterSize = size;
        tm.anchor = TextAnchor.MiddleCenter; tm.alignment = TextAlignment.Center;
        tm.color = color ?? Color.white;
    }

    private void BuildStreet()
    {
        Cube("Main asphalt street", new Vector3(0, -.15f, 0), new Vector3(12, .3f, 70), road);
        Cube("Left sidewalk", new Vector3(-7.2f, .02f, 0), new Vector3(2.4f, .25f, 70), sidewalk);
        Cube("Right sidewalk", new Vector3(7.2f, .02f, 0), new Vector3(2.4f, .25f, 70), sidewalk);
        for (int z = -32; z <= 32; z += 8)
        {
            Cube("Road marking", new Vector3(0, .012f, z), new Vector3(.12f, .025f, 3.2f), cream, false);
        }
        Cube("Road curb left", new Vector3(-6.05f, .15f, 0), new Vector3(.16f, .3f, 70), cream);
        Cube("Road curb right", new Vector3(6.05f, .15f, 0), new Vector3(.16f, .3f, 70), cream);
    }

    private void BuildBuildings()
    {
        for (int i = 0; i < 5; i++)
        {
            float z = -26 + i * 13;
            float h = 6 + (i % 3) * 1.6f;
            Cube("Apartment block left " + i, new Vector3(-10.3f, h / 2, z), new Vector3(4.2f, h, 11), i % 2 == 0 ? wall : cream);
            Cube("Apartment block right " + i, new Vector3(10.3f, h / 2, z + 4), new Vector3(4.2f, h, 10), i % 2 == 0 ? cream : wall);
            for (int floor = 1; floor < 3; floor++)
            for (int win = 0; win < 2; win++)
            {
                Cube("Window", new Vector3(-8.17f, floor * 2f + .5f, z - 2 + win * 3.5f), new Vector3(.04f, .9f, 1.2f), glass, false);
                Cube("Window", new Vector3(8.17f, floor * 2f + .5f, z + 2 + win * 3f), new Vector3(.04f, .9f, 1.2f), glass, false);
            }
        }
    }

    private void BuildCyberCafe()
    {
        // Open-front shop facing the street, at the right side.
        Cube("Cyber floor", new Vector3(4.8f, .18f, 1.8f), new Vector3(4.1f, .25f, 6.5f), sand);
        Cube("Cyber back wall", new Vector3(4.8f, 1.8f, 5.0f), new Vector3(4.1f, 3.3f, .25f), wall);
        Cube("Cyber left wall", new Vector3(2.75f, 1.8f, 1.8f), new Vector3(.22f, 3.3f, 6.5f), wall);
        Cube("Cyber right wall", new Vector3(6.85f, 1.8f, 1.8f), new Vector3(.22f, 3.3f, 6.5f), wall);
        Cube("Cyber sign board", new Vector3(4.8f, 3.8f, -1.35f), new Vector3(4.2f, .65f, .18f), sign);
        Label("سايبر TEPLINK", new Vector3(4.8f, 3.8f, -1.47f), .24f, Color.cyan);

        for (int i = 0; i < 3; i++)
        {
            float z = -.2f + i * 1.6f;
            Cube("Computer desk " + (i + 1), new Vector3(4.8f, .85f, z), new Vector3(2.8f, .16f, .65f), dark);
            Cube("Monitor " + (i + 1), new Vector3(4.8f, 1.45f, z + .1f), new Vector3(.85f, .58f, .12f), blue);
            Cube("Keyboard " + (i + 1), new Vector3(4.8f, .97f, z - .18f), new Vector3(.48f, .04f, .18f), dark, false);
            var trigger = new GameObject("PC interact " + (i + 1));
            trigger.transform.SetParent(transform);
            trigger.transform.position = new Vector3(4.8f, 1.35f, z + .35f);
            var box = trigger.AddComponent<BoxCollider>();
            box.size = new Vector3(1.15f, 1.2f, .8f);
            box.isTrigger = true;
            trigger.AddComponent<TEPLINKComputer>().computerNumber = i + 1;
        }
        Cube("Shop counter", new Vector3(3.3f, .65f, 4.1f), new Vector3(.7f, 1.2f, 1.3f), red);
    }

    private void BuildHome()
    {
        // Small open-front starter apartment, on the opposite end of the street.
        Cube("Home floor", new Vector3(-3.5f, .18f, -10.5f), new Vector3(4.2f, .25f, 5.2f), cream);
        Cube("Home back wall", new Vector3(-3.5f, 1.7f, -13.1f), new Vector3(4.2f, 3.1f, .2f), wall);
        Cube("Home left wall", new Vector3(-5.6f, 1.7f, -10.5f), new Vector3(.2f, 3.1f, 5.2f), wall);
        Cube("Home right wall", new Vector3(-1.4f, 1.7f, -10.5f), new Vector3(.2f, 3.1f, 5.2f), wall);
        Cube("Bed frame", new Vector3(-4.7f, .45f, -11.8f), new Vector3(1.2f, .4f, 1.8f), blue);
        Cube("Bed mattress", new Vector3(-4.7f, .7f, -11.8f), new Vector3(1.18f, .18f, 1.75f), cream);
        var bed = new GameObject("Bed interaction");
        bed.transform.position = new Vector3(-4.2f, 1f, -10.8f);
        bed.AddComponent<BoxCollider>().size = new Vector3(1.8f, 1.8f, 1.8f);
        bed.AddComponent<TEPLINKBed>();
        Cube("Home sign", new Vector3(-3.5f, 3.25f, -7.85f), new Vector3(3.3f, .45f, .12f), sign);
        Label("بيت صاحب السايبر", new Vector3(-3.5f, 3.25f, -7.94f), .16f, Color.white);
    }

    private void BuildServices()
    {
        // Foul/taameya kiosk with interactable food counter.
        Cube("Foul shop", new Vector3(-4.6f, 1.35f, 5.5f), new Vector3(3.1f, 2.7f, 2.8f), green);
        Cube("Foul sign", new Vector3(-4.6f, 2.95f, 4.05f), new Vector3(3.2f, .55f, .12f), sign);
        Label("فول وطعمية", new Vector3(-4.6f, 2.95f, 3.96f), .18f, Color.yellow);
        var food = new GameObject("Food interaction");
        food.transform.position = new Vector3(-3.7f, 1f, 3.5f);
        food.AddComponent<BoxCollider>().size = new Vector3(1.8f, 2f, 1.5f);
        food.AddComponent<TEPLINKFoodSpot>();

        // Drinks kiosk.
        Cube("Kiosk", new Vector3(-4.6f, 1.1f, 13f), new Vector3(3.1f, 2.2f, 2.8f), red);
        Cube("Kiosk sign", new Vector3(-4.6f, 2.5f, 11.55f), new Vector3(3.2f, .5f, .12f), sign);
        Label("مشروبات", new Vector3(-4.6f, 2.5f, 11.46f), .18f, Color.white);
        var drink = new GameObject("Drink interaction");
        drink.transform.position = new Vector3(-3.7f, 1f, 11f);
        drink.AddComponent<BoxCollider>().size = new Vector3(1.8f, 2f, 1.5f);
        drink.AddComponent<TEPLINKDrinkSpot>();

        // Two simplified fuel stations farther down the road.
        BuildFuelStation(-4.2f, 25f, "محطة بنزين ١");
        BuildFuelStation(4.2f, -25f, "محطة بنزين ٢");
    }

    private void BuildFuelStation(float x, float z, string title)
    {
        Cube(title + " canopy", new Vector3(x, 3f, z), new Vector3(4.8f, .3f, 5.5f), red);
        Cube(title + " pillar A", new Vector3(x - 1.8f, 1.5f, z - 2f), new Vector3(.25f, 3f, .25f), cream);
        Cube(title + " pillar B", new Vector3(x + 1.8f, 1.5f, z + 2f), new Vector3(.25f, 3f, .25f), cream);
        Cube(title + " pump", new Vector3(x, .8f, z), new Vector3(.65f, 1.6f, .6f), dark);
        Label(title, new Vector3(x, 3.45f, z - 2.4f), .13f, Color.white);
    }

    private void BuildDecor()
    {
        // A parked, boxy Egyptian street car as a visual landmark.
        Cube("Parked car body", new Vector3(-2.5f, .65f, 18f), new Vector3(1.7f, .7f, 3.2f), blue);
        Cube("Parked car cabin", new Vector3(-2.5f, 1.15f, 18f), new Vector3(1.35f, .55f, 1.5f), glass);
        for (int i = -1; i <= 1; i += 2)
        for (int j = -1; j <= 1; j += 2)
        {
            var wheel = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            wheel.name = "Car wheel"; wheel.transform.SetParent(transform);
            wheel.transform.position = new Vector3(-2.5f + i * .88f, .35f, 18f + j * .95f);
            wheel.transform.rotation = Quaternion.Euler(0, 0, 90);
            wheel.transform.localScale = new Vector3(.42f, .16f, .42f);
            wheel.GetComponent<Renderer>().material = dark;
        }

        // Street lamps.
        for (int z = -25; z <= 25; z += 12)
        {
            Cube("Street lamp pole", new Vector3(-6.8f, 2.1f, z), new Vector3(.12f, 4.2f, .12f), dark);
            Cube("Street lamp head", new Vector3(-6.55f, 4.15f, z), new Vector3(.7f, .12f, .22f), cream);
        }
    }

    private void BuildNPCs()
    {
        for (int i = 0; i < 4; i++)
        {
            var npc = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            npc.name = "Neighborhood pedestrian " + (i + 1);
            npc.transform.SetParent(transform);
            npc.transform.position = new Vector3(i % 2 == 0 ? -6.8f : 6.8f, 1f, -16 + i * 9);
            npc.GetComponent<Renderer>().material = i % 2 == 0 ? blue : green;
            npc.AddComponent<TEPLINKPedestrian>();
        }
    }
}
