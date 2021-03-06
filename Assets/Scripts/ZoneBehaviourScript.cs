﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZoneBehaviourScript : MonoBehaviour
{
    SpriteRenderer zoneImage;
    GameController gc;
    // Start is called before the first frame update
    void Start()
    {
        MapBehavior map = GameObject.Find("MapController").GetComponent<MapBehavior>();
        gc = GameObject.Find("GameController").GetComponent<GameController>();
        int size = map.GetMapSize();
        Vector3 zoneScale = new Vector3(size / 5, size / 5, 1);
        zoneImage = GetComponent<SpriteRenderer>();
        transform.localScale = zoneScale;
        map.zonePos = transform.position;
        map.zoneRadius = transform.localScale.x * 1.5f;
        StartCoroutine("Fade");
        StartCoroutine("IsUnitInCircle");
    }
    IEnumerator IsUnitInCircle()
    {
        while (true)
        {
            Collider2D[] unitsWithinRange = Physics2D.OverlapCircleAll(transform.position, transform.localScale.x * 1.5f, ~(1 << 8));
            foreach (Collider2D unit in unitsWithinRange)
            {
                //change to two different tags depending on team and give score accordingly
                if (unit.tag == "Player" && unit.GetComponent<PlayerBehaviour>().GetTeam() == 1)
                {
                    gc.GiveScoreToTeamOne(10);
                }
                else if (unit.tag == "Player") {
                    gc.GiveScoreToTeamTwo(10);
                }
            }
            yield return new WaitForSeconds(5);
        }
    }
    IEnumerator Fade()
    {
        float lerpAmount = 0;
        bool fadeOut = true;
        while (true)
        {
            Color c = zoneImage.color;
            if (c.a <= 0.1f && fadeOut)
            {
                fadeOut = false;
                lerpAmount = 0;
            }
            else if (c.a >= 0.7f && !fadeOut)
            {
                fadeOut = true;
                lerpAmount = 0;
            }
            if (fadeOut)
            {
                c.a = Mathf.Lerp(0.7f, 0.1f, lerpAmount);
                zoneImage.color = c;
                lerpAmount += 0.01f;
            }
            else
            {
                c.a = Mathf.Lerp(0.1f, 0.7f, lerpAmount);
                zoneImage.color = c;
                lerpAmount += 0.01f;
            }
            yield return null;
        }
    }
}
