using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUtility
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T">랭크에 저장하고 비교할 대상</typeparam>
    /// <typeparam name="S">순위를 비교하기 위한 값</typeparam>
    public interface IRank<T, S> where S : IComparable<S>
    {
        /// <summary>
        /// 1순위 값을 리턴
        /// </summary>
        T One();
        /// <summary>
        /// 1순위가 존재하는지 확인
        /// </summary>
        /// <returns>1순위의 존재 여부</returns>
        bool HasOne();

        /// <summary>
        /// 해당 순위가 존재한다면 output에 순위 값을 전달
        /// </summary>
        /// <param name="value">순위</param>
        /// <param name="output">해당 순위의 값 리턴</param>
        bool TryGetRank(int value, out T output);
        /// <summary>
        /// 해당 순위가 존재한다면 true를 리턴하고 output에 순위 값을 전달
        /// 해당 순위가 존재하지 않다면 false를 리턴
        /// </summary>
        /// <param name="value">순위</param>
        /// <param name="output">해당 순위의 값 리턴</param>
        T GetRank(int value);
        S this[T index] { get; set; }
    }
}
