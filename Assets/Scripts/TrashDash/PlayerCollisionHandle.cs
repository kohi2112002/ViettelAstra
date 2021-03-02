using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerCollisionHandle : MonoBehaviour
{
    public PlayerController playerController;
    private bool invincible = false;
    private void OnEnable()
    {
        Shader.SetGlobalFloat("_BlinkingValue", 0.0f);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            other.gameObject.SendMessage("PlayCollisionAnim");
            if (!invincible)
                playerController.OnPlayerDie();
            StartCoroutine(InvincibleTimer(3));
        }
    }
    protected IEnumerator InvincibleTimer(float timer)
    {
        invincible = true;

        float time = 0;
        float currentBlink = 1.0f;
        float lastBlink = 0.0f;
        const float blinkPeriod = 0.1f;

        while (time < timer && invincible)
        {
            Shader.SetGlobalFloat("_BlinkingValue", currentBlink);

            // We do the check every frame instead of waiting for a full blink period as if the game slow down too much
            // we are sure to at least blink every frame.
            // If blink turns on and off in the span of one frame, we "miss" the blink, resulting in appearing not to blink.
            yield return null;
            time += Time.deltaTime;
            lastBlink += Time.deltaTime;

            if (blinkPeriod < lastBlink)
            {
                lastBlink = 0;
                currentBlink = 1.0f - currentBlink;
            }
        }
        Shader.SetGlobalFloat("_BlinkingValue", 0.0f);
        invincible = false;
    }
}
