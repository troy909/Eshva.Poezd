using System.Collections.Generic;


namespace Eshva.Common
{
  public interface IPocket
  {
    /// <exception cref="System.ArgumentNullException">
    /// Не задан ключ искомого значения.
    /// </exception>
    /// <exception cref="System.InvalidCastException">
    /// Найденное значение имеет тип, отличный от ожидаемого.
    /// </exception>
    bool TryGet<TValue>(out TValue value);

    /// <exception cref="System.ArgumentNullException">
    /// Не задан ключ искомого значения.
    /// </exception>
    /// <exception cref="System.InvalidCastException">
    /// Найденное значение имеет тип, отличный от ожидаемого.
    /// </exception>
    bool TryGet<TValue>(string key, out TValue value);

    /// <exception cref="System.ArgumentNullException">
    /// Значение не задано.
    /// </exception>
    void Set<TValue>(TValue value);

    /// <exception cref="System.ArgumentNullException">
    /// Не задан ключ или значение, либо ключ представляет собой пустую строку или только пробельные символы.
    /// </exception>
    void Set<TValue>(string key, TValue value);

    bool TryRemove<TValue>();

    /// <exception cref="System.ArgumentNullException">
    /// Ключ представляет собой пустую строку или только пробельные символы.
    /// </exception>
    bool TryRemove(string key);

    IEnumerable<KeyValuePair<string, object>> GetItems();
  }
}
