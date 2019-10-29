using System.Threading.Tasks;

namespace TE.ComponentLibrary.ComponentLibrary.ComponentMasters.Infrastructure.Repository
{
    /// <summary>
    /// Repository method for counter
    /// </summary>
    public interface ICounterRepository
    {
        /// <summary>
        /// Gets the nexts value.
        /// </summary>
        /// <param name="counterId">The counter identifier.</param>
        /// <returns></returns>
        Task<int> NextValue(string counterId);

        /// <summary>
        /// Updates the specified counter value.
        /// </summary>
        /// <param name="counterValue">The counter value.</param>
        /// <param name="counterId">The counter identifier.</param>
        /// <returns></returns>
        Task Update(int counterValue, string counterId);

        /// <summary>
        /// Gets the current value.
        /// </summary>
        /// <param name="counterId">The counter identifier.</param>
        /// <returns></returns>
        Task<int> CurrentValue(string counterId);
    }
}