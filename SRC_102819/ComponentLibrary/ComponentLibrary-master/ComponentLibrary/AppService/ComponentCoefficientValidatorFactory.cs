using System;
using TE.ComponentLibrary.ComponentLibrary.ControllerAdaptors.DTOs;
using TE.ComponentLibrary.ComponentLibrary.RepositoryPort;
using TE.ComponentLibrary.ComponentLibrary.ServiceMasters.Infrastructure.Repository;

namespace TE.ComponentLibrary.ComponentLibrary.AppService
{
	/// <summary>
	/// </summary>
	public class ComponentCoefficientValidatorFactory : IComponentCoefficientValidatorFactory
	{
		private readonly IMaterialRepository _materialRepository;
		private readonly IServiceRepository _serviceRepository;
		private readonly ICompositeComponentRepository _semiFinishedGoodRepository;

		/// <summary>
		/// Initializes a new instance of the <see cref="ComponentCoefficientValidatorFactory"/> class.
		/// </summary>
		/// <param name="materialRepository">The material repository.</param>
		/// <param name="serviceRepository">The service repository.</param>
		/// <param name="semiFinishedGoodRepository">The semi finished good repository.</param>
		public ComponentCoefficientValidatorFactory(IMaterialRepository materialRepository,
			IServiceRepository serviceRepository, ICompositeComponentRepository semiFinishedGoodRepository)
		{
			_materialRepository = materialRepository;
			_serviceRepository = serviceRepository;
			_semiFinishedGoodRepository = semiFinishedGoodRepository;
		}

		/// <summary>
		/// Gets the component coefficient validator.
		/// </summary>
		/// <param name="componentType">Type of the component.</param>
		/// <returns></returns>
		/// <exception cref="System.NotImplementedException"></exception>
		public IComponentCoefficientValidator GetComponentCoefficientValidator(ComponentType componentType)
		{
			switch (componentType)
			{
				case ComponentType.Material:
					return new MaterialCoefficientValidator(_materialRepository);

				case ComponentType.Service:
					return new ServiceCoefficientValidator(_serviceRepository);

				case ComponentType.Asset:
					return new AssetCoefficientValidator(_materialRepository);

				case ComponentType.SFG:
					return new SemiFinishedGoodCoefficientValidator(_semiFinishedGoodRepository);

				default:
					throw new NotImplementedException(componentType + " is not implemented.");
			}
		}
	}
}