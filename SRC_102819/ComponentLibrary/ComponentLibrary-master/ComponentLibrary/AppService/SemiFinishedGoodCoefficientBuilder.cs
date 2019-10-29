using System;
using System.Threading.Tasks;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	/// <seealso cref="TE.ComponentLibrary.ComponentLibrary.AppService.IComponentCoefficientBuilder"/>
	public class SemiFinishedGoodCoefficientBuilder : IComponentCoefficientBuilder
	{
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _semiFinishedGoodDefinitionRepository;
		private readonly ICompositeComponentRepository _semiFinishedGoodRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="SemiFinishedGoodCoefficientBuilder"/> class.
		/// </summary>
		/// <param name="semiFinishedGoodRepository">The semi finished good repository.</param>
		/// <param name="semiFinishedGoodDefinitionRepository">
		/// The semi finished good definition repository.
		/// </param>
		public SemiFinishedGoodCoefficientBuilder(ICompositeComponentRepository semiFinishedGoodRepository,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> semiFinishedGoodDefinitionRepository)
		{
			_semiFinishedGoodRepository = semiFinishedGoodRepository;
			_semiFinishedGoodDefinitionRepository = semiFinishedGoodDefinitionRepository;
		}

		/// <summary>
		/// Builds the data.
		/// </summary>
		/// <param name="componentCoefficient">The component coefficient.</param>
		/// <returns></returns>
		public async Task<ComponentCoefficient> BuildData(ComponentCoefficient componentCoefficient)
		{
			var sfgDefintion = await _semiFinishedGoodDefinitionRepository.Find("sfg", Keys.Sfg.SfgDefinitionGroup);
			var semiFinishedGood = await _semiFinishedGoodRepository.Find("sfg", componentCoefficient.Code, sfgDefintion);
			if (semiFinishedGood == null)
				throw new ArgumentException($"Invalid semi finished good code. {componentCoefficient.Code}");
			componentCoefficient.Name = (string)semiFinishedGood["General"]["Short Description"];
			componentCoefficient.UnitOfMeasure = (string)semiFinishedGood["General"]["Unit Of Measure"];
			return componentCoefficient;
		}
	}
}