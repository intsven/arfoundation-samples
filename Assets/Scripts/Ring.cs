using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Radar
{
	public class Ring : MonoBehaviour
	{
		public Options Opts;

		private float radius;
		private Vector3[] ringTemplate;

		void Start ()
		{
			this.radius = this.Opts.StartRadius;

			this.ringTemplate = makeTemplate (this.Opts.Segments);
			Mesh mesh = buildRing (this.Opts.Segments);
			GetComponent<MeshFilter> ().mesh = mesh;
		}

		void Update ()
		{
			if (this.radius - this.Opts.Width > this.Opts.EndRadius) {
				Destroy (gameObject);
			}

			this.radius += this.Opts.Rate * Time.deltaTime;
			float outer = Mathf.Clamp (this.radius, this.Opts.StartRadius, this.Opts.EndRadius);
			float inner = Mathf.Clamp (this.radius - this.Opts.Width, this.Opts.StartRadius, this.Opts.EndRadius);

			Mesh mesh = GetComponent<MeshFilter> ().mesh;
			mesh.vertices = setRadius (mesh.vertices, ringTemplate, inner, outer);
		}

		Vector3[] makeTemplate(int segments) {
			Vector3[] template = new Vector3[segments];
			for (int i = 0; i < template.Length; i++) {
				float angle = i * 2 * Mathf.PI / segments;
				template [i] = new Vector3 (Mathf.Sin (angle), 0, Mathf.Cos (angle));
			}
			return template;
		}

		Mesh buildRing(int Segments) {
			Mesh mesh = new Mesh ();
			GetComponent<MeshFilter> ().mesh = mesh;

			mesh.Clear ();

			Vector3[] vertices = new Vector3[2 * Segments];
			Vector3[] normals = new Vector3[2 * Segments];
			int[] triangles = new int[2 * 3 * Segments];

			for (int i = 0; i < Segments; i += 1) {
				triangles [6 * i + 0] = (i + 0) % Segments;
				triangles [6 * i + 1] = (i + 1) % Segments;
				triangles [6 * i + 2] = (i + 0) % Segments + Segments;

				triangles [6 * i + 3] = (i + 0) % Segments + Segments;
				triangles [6 * i + 4] = (i + 1) % Segments;
				triangles [6 * i + 5] = (i + 1) % Segments + Segments;			
			}

			for (int i = 0; i < normals.Length; i++) {
				normals [i] = Vector3.up;
			}

			mesh.vertices = vertices;
			mesh.normals = normals;
			mesh.triangles = triangles;

			return mesh;
		}

		Vector3[] setRadius(Vector3[] vertices, Vector3[] template, float inner, float outer) {
			for (int i = 0; i < template.Length; i++) {
				vertices [i] = template [i] * outer;
				vertices [i + template.Length] = template [i] * inner;
			}

			return vertices;
		}
	}
}