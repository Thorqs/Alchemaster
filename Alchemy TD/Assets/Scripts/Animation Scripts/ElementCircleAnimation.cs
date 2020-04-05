using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementCircleAnimation : MonoBehaviour {

    public GameObject ElementCircle;
    public GameObject MiddleLine;
    public GameObject FarLeftLine;
    public GameObject LeftTopLine;
    public GameObject LeftBottomLine;
    public GameObject FarRightLine;
    public GameObject RightTopLine;
    public GameObject RightBottomLine;

    public float normalCircleSize;
    public float reducedCircleSize;

    public void deactivateLines()
    {
        var particles = ElementCircle.GetComponent<ParticleSystem>().main;
        particles.startSize = normalCircleSize;

        ElementCircle.SetActive(false);
        MiddleLine.SetActive(false);
        FarLeftLine.SetActive(false);
        LeftTopLine.SetActive(false);
        LeftBottomLine.SetActive(false);
        FarRightLine.SetActive(false);
        RightTopLine.SetActive(false);
        RightBottomLine.SetActive(false);
    }

    public void highlight()
    {
        ElementCircle.SetActive(true);
        var particles = ElementCircle.GetComponent<ParticleSystem>().main;
        particles.startSize = reducedCircleSize;
    }

    public void ConnectSelf()
    {
        deactivateLines();

        ElementCircle.SetActive(true);
        MiddleLine.SetActive(true);
        LeftTopLine.SetActive(true);
        RightTopLine.SetActive(true);
    }

    public void ConnectLeftClose()
    {
        deactivateLines();

        ElementCircle.SetActive(true);
        FarLeftLine.SetActive(true);
        LeftTopLine.SetActive(true);
    }

    public void ConnectLeftFar()
    {
        deactivateLines();

        ElementCircle.SetActive(true);
        LeftTopLine.SetActive(true);
        LeftBottomLine.SetActive(true);
    }

    public void ConnectRightClose()
    {
        deactivateLines();

        ElementCircle.SetActive(true);
        FarRightLine.SetActive(true);
        RightTopLine.SetActive(true);
    }

    public void ConnectRightFar()
    {
        deactivateLines();

        ElementCircle.SetActive(true);
        RightTopLine.SetActive(true);
        RightBottomLine.SetActive(true);
    }
}
