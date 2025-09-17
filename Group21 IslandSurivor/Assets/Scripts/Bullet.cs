using UnityEngine;
using UnityEngine.SceneManagement;

public class Bullet : MonoBehaviour
{
    public float damage = 50f;
    public GameObject owner;

    void Start()
    {
        // CANCEL any DontDestroyOnLoad - FORCE into current scene
        if (gameObject.scene.name == "DontDestroyOnLoad")
        {
            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());
        }

        // GUARANTEED destruction after time
        Destroy(gameObject, 2f);
    }

    void Update()
    {
        // SAFETY CHECK: If somehow in DontDestroyOnLoad scene, destroy immediately
        if (gameObject.scene.name == "DontDestroyOnLoad")
        {
            Debug.LogError("Bullet still in DontDestroyOnLoad - destroying!");
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == owner) return;

        SimpleEnemy enemy = collision.gameObject.GetComponent<SimpleEnemy>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        Destroy(gameObject);
    }
}