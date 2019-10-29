using System;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.Domain;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public class ComponentCoefficientBuilderFactory : IComponentCoefficientBuilderFactory
	{
		private readonly IMaterialRepository _materialRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly ICompositeComponentRepository _semiFinishedGoodRepository;
		private readonly ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> _semiFinishedGoodDefinitionRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentCoefficientBuilderFactory"/> class.
		/// </summary>
		/// <param name="materialRepository">The material repository.</param>
		/// <param name="serviceRepository">The service repository.</param>
		/// <param name="semiFinishedGoodRepository">The semi finished good repository.</param>
		/// <param name="semiFinishedGoodDefinitionRepository">
		/// The semi finished good definition repository.
		/// </param>
		public ComponentCoefficientBuilderFactory(IMaterialRepository materialRepository, IServiceRepository serviceRepository,
			ICompositeComponentRepository semiFinishedGoodRepository,
			ICompositeComponentDefinitionRepository<ICompositeComponentDefinition> semiFinishedGoodDefinitionRepository)
		{
			_materialRepository = materialRepository;
			_serviceRepository = serviceRepository;
			_semiFinishedGoodRepository = semiFinishedGoodRepository;
			_semiFinishedGoodDefinitionRepository = semiFinishedGoodDefinitionRepository;
		}

		/// <summary>
		/// Gets the component coefficient builder.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns></returns>
		public IComponentCoefficientBuilder GetComponentCoefficientBuilder(ComponentType componentType)
		{
			switch (componentType)
			{
				case ComponentType.Material:
					return new MaterialCoefficientBuilder(_materialRepository);

				case ComponentType.Service:
					return new ServiceCoefficientBuilder(_serviceRepository);

				case ComponentType.Asset:
					return new AssetCoefficientBuilder(_materialRepository);

				case ComponentType.SFG:
					return new SemiFinishedGoodCoefficientBuilder(_semiFinishedGoodRepository, _semiFinishedGoodDefinitionRepository);

				default:
					throw new NotImplementedException(componentType + " is not implemented.");
			}
		}
	}
}