using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using UnityEngine;

public class ElectriclineTower : MonoBehaviour
{
    private BaseTower _baseTower;
    private ElectriclineTower _connectedTower;
    private GameObject _electricLine;

    public bool IsConnected => _connectedTower != null;

    public BaseTower GetBaseTower() => _baseTower;

    private void Start()
    {
        _baseTower = GetComponent<BaseTower>();
    }

    /// <summary>
    /// 다른 타워와 연결을 시도
    /// </summary>
    public bool TryConnectTo(ElectriclineTower other)
    {
        // 본체가 연결이 되어있거나, 연결할 대상이 전깃줄 타워가 아니거나, 해당 타워가 이미 연결된 타워이거나
        if (IsConnected || other == null || other.IsConnected)
            return false;

        // 양방향 등록
        _connectedTower = other;
        other._connectedTower = this;

        // 라인 긋기 
        _electricLine = LineConnectingManager.Instance.CreateLineBetween(this, other);
        return true;
    }

    /// <summary>
    /// 현재 연결을 해제하고 전깃줄 회수
    /// </summary>
    public void Disconnect()
    {
        if (_connectedTower != null)
        {
            var other = _connectedTower;
            _connectedTower = null;
            other._connectedTower = null;

            if (_electricLine != null)
            {
                LineConnectingManager.Instance.ReturnLineToPool(_electricLine);
                _electricLine = null;
            }
        }
    }

    private void OnDestroy()
    {
        Disconnect(); // 파괴 시 연결 자동 해제
    }
}
