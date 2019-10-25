using UnityEngine;

public class Example : MonoBehaviour {

    //Properties.
    public GameObject sphere;

    //Update.
    public void Update() {
        transform.parent.eulerAngles = new Vector3(transform.parent.eulerAngles.x, transform.parent.eulerAngles.y + (Time.deltaTime * 10),
                transform.parent.eulerAngles.z);
    }

    //Generate spheres.
    public void generateSphereOnLeft() {
        GameObject thisSphere = GameObject.Instantiate(sphere);
        thisSphere.transform.position = new Vector3(-11.09958f, 16, 3.5f);
    }
    public void generateSphereInCentre() {
        GameObject thisSphere = GameObject.Instantiate(sphere);
        thisSphere.transform.position = new Vector3(Random.Range(-0.01f, 0.01f), 16, Random.Range(-0.01f, 0.01f));
    }
    public void generateSphereOnRight() {
        GameObject thisSphere = GameObject.Instantiate(sphere);
        thisSphere.transform.position = new Vector3(11.09958f, 16, -3.5f);
    }
}