using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Sidewalk procedurally generates a sidewalk with the given attributes
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(BoxCollider))]
public class Sidewalk : MonoBehaviour {
    // The number of vertices needed to add a sidewalk segment
    private static int PIECES = 4;

    // The curb dimensions
    public Vector2 curb = new Vector2(.3f,.3f);
    // The sidewalk dimensions
    public Vector2 side = new Vector2(1f,1f);
    // Generate a corner at the end of a sidewalk
    public bool cornerEnd = false;
    // The number of sidewalk segments to generate
    [Range(1, 1000)]
    public int links = 1;

    // The mesh components
    private Mesh walk;
    private MeshFilter mf;

    // The sidewalk collider
    private BoxCollider coll;

	void Start () {
        // If the sidewalk was not generated in editor, generate the sidewalk
		if (walk == null)
        {
            Generate();
        }
	}

    // Generate the sidewalk mesh and resize the collider appropriately
    [ContextMenu("Generate Mesh")]
    private void Generate()
    {
        // Get the mesh components ready
        float z;
        if (walk == null)
        {
            walk = new Mesh();
        }
        else
        {
            walk.Clear();
        }

        if (mf == null)
        {
            mf = GetComponent<MeshFilter>();
        }

        if (coll == null)
        {
            coll = GetComponent<BoxCollider>();
        }

        // Vertices
        List<Vector3> verts = new List<Vector3>();

        // The beginning sidewalk vertices
        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0, curb.y, 0));
        verts.Add(new Vector3(curb.x, curb.y, 0));
        verts.Add(new Vector3(curb.x + side.x, curb.y, 0));

        // Generate the vertices for each segment
        for (int ind = 0; ind < links; ind++)
        {
            z = (ind + 1) * side.y;
            verts.Add(new Vector3(0, 0, z));
            verts.Add(new Vector3(0, curb.y, z));
            verts.Add(new Vector3(curb.x, curb.y, z));
            verts.Add(new Vector3(curb.x + side.x, curb.y, z));
        }

        // Generate the vertices for the end piece
        if (cornerEnd)
        {
            z = links * side.y + side.x + curb.x;
            verts.Add(new Vector3(0,0, z));
            verts.Add(new Vector3(0, curb.y, z));
            verts.Add(new Vector3(curb.x, curb.y, z));
            verts.Add(new Vector3(curb.x + side.x, curb.y, z));
            verts.Add(new Vector3(curb.x, 0, z));
            verts.Add(new Vector3(curb.x + side.x, 0, z));
        }

        // Set the mesh vertices
        walk.SetVertices(verts);

        // Generate the triangles for all of the sidewalk segments
        List<int> tris = new List<int>();
        for (int ind = 0; ind < links; ind++)
        {
            for (int ve = 0; ve < PIECES - 1; ve++)
            {
                int start = ind * PIECES + ve;
                tris.Add(start);
                tris.Add(start + PIECES);
                tris.Add(start + 1);
                tris.Add(start + PIECES);
                tris.Add(start + PIECES + 1);
                tris.Add(start + 1);
            }
        }

        // Generate the triangles for the sidewalk corner
        if (cornerEnd)
        {
            for (int ve = 0; ve < PIECES - 1; ve++)
            {
                int strt = links * PIECES + ve;
                tris.Add(strt);
                tris.Add(strt + PIECES);
                tris.Add(strt + 1);
                tris.Add(strt + PIECES);
                tris.Add(strt + PIECES + 1);
                tris.Add(strt + 1);
            }
            tris.Add(verts.Count - 6);
            tris.Add(verts.Count - 2);
            tris.Add(verts.Count - 5);
            tris.Add(verts.Count - 5);
            tris.Add(verts.Count - 2);
            tris.Add(verts.Count - 4);

            tris.Add(verts.Count - 2);
            tris.Add(verts.Count - 1);
            tris.Add(verts.Count - 4);
            tris.Add(verts.Count - 4);
            tris.Add(verts.Count - 1);
            tris.Add(verts.Count - 3);
        }

        // Set the triangles and calculate the normals
        walk.SetTriangles(tris,0);
        walk.RecalculateNormals();

        // Display the mesh with the filter
        mf.mesh = walk;

        // Resize the collider based on the sidewalk size
        float addY = 0f;
        if (cornerEnd)
        {
            addY = side.y + curb.x;
        }
        
        coll.size = new Vector3(curb.x + side.x, curb.y, links * side.y + addY);
        coll.center = new Vector3(coll.size.x / 2f, coll.size.y / 2, coll.size.z / 2);
    }
}
