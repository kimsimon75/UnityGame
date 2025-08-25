using System;
using System.Collections;
using System.Collections.Generic;

public class PriorityQueue<T> : IEnumerable<T> where T : IComparable<T>
{
    public T[] data;
    public int Count { get; private set; }
    public int Capacity { get; private set; }
    #region 생성자
    // 기본 생성자
    public PriorityQueue()
    {
        Count = 0;
        Capacity = 1;
        data = new T[Capacity];
    }
    // 초기 Capacity 지정 생성자
    public PriorityQueue(int capacity)
    {
        Count = 0;
        Capacity = capacity;
        data = new T[Capacity];
    }
    #endregion

    #region public function
    public void Enqueue(T value)
    {
        // data 배열이 꽉 찼다면 확장
        if (Count >= Capacity)
            Expand();
        // 데이터 추가
        data[Count] = value;
        Count++;

        // 힙 트리를 유지하기 위해 데이터 교환
        // 새로 추가한 노드부터 부모 노드와 비교하여 더 크다면 
        int now = Count - 1;
        while (now > 0)
        {
            int parent = (now - 1) / 2;
            // 부모 노드의 값이 더 크다면 정지
            if (data[now].CompareTo(data[parent]) < 0)
                break;

            // 값 교환
            T temp = data[now];
            data[now] = data[parent];
            data[parent] = temp;
            // 현재 위치 갱신
            now = parent;
        }

    }

    public T Dequeue()
    {
        // Count가 0이라면 예외 발생
        if (Count == 0)
            throw new IndexOutOfRangeException();

        // 루트 노드 값 추출
        // 마지막 노드와 교환 후 제거
        T result = data[0];
        data[0] = data[Count - 1];
        data[Count - 1] = default(T);
        Count--;

        // 힙 트리를 유지하도록 데이터 교환
        // 루트부터 시작하여 자식 노드 중 큰 쪽과 비교, 현재 노드가 더 작다면 교환
        int now = 0;
        while (now < Count)
        {
            int left = (now * 2) + 1;
            int right = (now * 2) + 2;

            int next = now;
            // 왼쪽 노드가 존재하고 값이 더 크다면 next 갱신 
            if (left < Count && data[next].CompareTo(data[left]) < 0)
                next = left;
            // 오른쪽 노드가 존재하고 값이 더 크다면 next 갱신 
            if (right < Count && data[next].CompareTo(data[right]) < 0)
                next = right;
            // 갱신되지 않았다면 루프 종료
            if (next == now)
                break;

            // 값 교환
            T temp = data[now];
            data[now] = data[next];
            data[next] = temp;
            // 현재 위치 갱신
            now = next;
        }

        return result;
    }

    public T Peek()
    {
        // Count가 0이라면 예외 발생
        if (Count == 0)
            throw new IndexOutOfRangeException();

        return data[0];
    }

    public bool RemoveAt(int i)
    {
        if (i < 0 || i >= Count) return false;

        int last = Count - 1;
        if (i == last)
        {
            data[last] = default;
            Count--;
            return true;
        }

        // i 위치에 마지막 노드를 가져오고 마지막을 비움
        T replaced = data[last];
        data[i] = replaced;
        data[last] = default;
        Count--;

        // 힙 복구: Max-heap 기준
        // 부모보다 크면 위로, 아니면 아래로
        int parent = (i - 1) / 2;
        if (i > 0 && data[i].CompareTo(data[parent]) > 0)
        {
            // sift up
            int now = i;
            while (now > 0)
            {
                int p = (now - 1) / 2;
                if (data[now].CompareTo(data[p]) <= 0) break;
                (data[now], data[p]) = (data[p], data[now]);
                now = p;
            }
        }
        else
        {
            // sift down
            int now = i;
            while (true)
            {
                int l = now * 2 + 1, r = now * 2 + 2, nxt = now;
                if (l < Count && data[nxt].CompareTo(data[l]) < 0) nxt = l;
                if (r < Count && data[nxt].CompareTo(data[r]) < 0) nxt = r;
                if (nxt == now) break;
                (data[now], data[nxt]) = (data[nxt], data[now]);
                now = nxt;
            }
        }
        return true;
    }

    // 값으로 지우기 (첫 매치만). 찾는 데 O(n), 지우기는 O(log n)
    public bool Remove(T value)
    {
        var eq = EqualityComparer<T>.Default;
        for (int i = 0; i < Count; i++)
            if (eq.Equals(data[i], value))
                return RemoveAt(i);
        return false;
    }

    #endregion

    #region private function
    // data 배열 확장용
    // 기존 Capacity의 2배로 확장
    void Expand()
    {
        T[] newData = new T[Capacity * 2];
        for (int i = 0; i < Count; i++)
            newData[i] = data[i];

        data = newData;
        Capacity *= 2;
    }

    public IEnumerator<T> GetEnumerator()
    {
        for (int i = 0; i < Count; i++)
            yield return data[i];
    }
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    // ▼ 우선순위(큰 값→작은 값) 순서로 안전하게 열거하고 싶다면 (원본 보존)
    public IEnumerable<T> EnumerateByPriority()
    {
        var copy = new PriorityQueue<T>(Count);
        // 힙 그대로 복사 (동일 타입이므로 private 접근 가능)
        Array.Copy(this.data, copy.data, this.Count);
        copy.Count = this.Count;
        // 이제 복사본에서 꺼내며 yield
        while (copy.Count > 0)
            yield return copy.Dequeue();
    }
    #endregion
}