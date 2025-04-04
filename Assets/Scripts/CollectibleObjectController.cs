using UnityEngine;

public class CollectibleController : GameObjectController
{    
    [SerializeField] private int coinValue = 1;
    [SerializeField] private ParticleSystem collectEffect;
    protected override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OnDespawn();
            // Add coins to the CoinManager
            if (CoinManager.instance != null)
            {
                CoinManager.instance.AddCoins(coinValue);
            }
            
            // Play collection effect if available
            if (collectEffect != null)
            {
                ParticleSystem effect = Instantiate(collectEffect, transform.position, Quaternion.identity);
                effect.Play();
                Destroy(effect.gameObject, effect.main.duration);
            }
            
        }
    }
}