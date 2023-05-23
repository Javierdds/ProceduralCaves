using Player;
using UnityEngine;

public class InteractiveCoin : InteractiveObject
{
    [SerializeField] private GameObject _coinGO;
    [SerializeField] private int _points;

    public override void Interact()
    {
        throw new System.NotImplementedException();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<PlayerManager>().PointsController.AddPoints(_points);
            DisableObject();
        }
    }

    public void DisableObject()
    {
        _coinGO.SetActive(false);
    }
}
