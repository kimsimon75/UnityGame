// Assets/Scripts/UiRayUtil.cs
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public static class UiRayUtil
{
    static readonly List<RaycastResult> _results = new List<RaycastResult>(16);

    /// ignoreMask에 포함된 레이어는 건너뛰고,
    /// 그 다음 "맨 위" UI가 있으면 true(차단), 없으면 false(통과)
    public static bool IsPointerOverUIExcept(LayerMask ignoreMask)
    {
        var es = EventSystem.current;
        if (!es) return false;

        var ped = new PointerEventData(es) { position = Input.mousePosition };
        _results.Clear();
        es.RaycastAll(ped, _results);     // 정렬되어 옴(맨 위가 index 0)

        for (int i = 0; i < _results.Count; i++)
        {
            var go = _results[i].gameObject;
            int bit = 1 << go.layer;

            if ((ignoreMask.value & bit) != 0)
                continue;                  // 무시할 레이어면 건너뜀

            return true;                   // 무시 대상이 아닌 "맨 위" UI가 있음 = 차단
        }
        return false;                      // 무시 대상만 있었거나 UI가 없음 = 통과
    }
}
