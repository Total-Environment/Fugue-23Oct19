import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/package_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('package Connector', () => {
	describe('mapStateToProps', () => {
		it('should return current package details with packageCode as parameter', () => {
			const state = {
				reducer: {
					components: {"PACKAGE0001": 2},
					packageCosts: {'PACKAGE0001': 'package costs'},
					error: '',
					classificationDefinition: {"Classification definition": "Classification definition"}
				}
			};
			const props = {params: {packageCode: 'PACKAGE0001'}};
			const actual = mapStateToProps(state, props);
			const expected = {
				packageCode: 'PACKAGE0001',
				details: 2,
				packageCostError: undefined,
				cost: 'package costs',
				error: '',
			};
			expect(actual).to.deep.equal(expected);
		});
	});

	describe('mapDispatchToProps', () => {
		describe('onPackageCostFetchRequest', () => {
			let dispatchSpy, returnedObject, props;
			// beforeEach(() => {
			// 	dispatchSpy = sinon.spy();
			// 	props = {params: {packageCode: 'PACKAGE0001'}};
			// 	returnedObject = mapDispatchToProps(dispatchSpy, props);
			// });

			it('should dispatch PACKAGE_COST_FETCH_SUCCEEDED when package cost exists', async () => {
        dispatchSpy = sinon.spy();
        	props = {params: {packageCode: 'PACKAGE0001'}};
        	returnedObject = mapDispatchToProps(dispatchSpy, props);
				ConnectorRewired.__Rewire__('axios', {
					get: sinon.stub().returns(Promise.resolve({
						data: {
							cost: 'cost'
						}
					}))
				});
				await returnedObject.onPackageCostFetchRequest();
				expect(dispatchSpy).to.be.calledWith({
					type: 'PACKAGE_COST_FETCH_SUCCEEDED',
					cost: {cost: "cost"},
					packageCode: 'PACKAGE0001'
				});
			});


			it('should dispatch PACKAGE_COST_FETCH_FAILED when package does not exists', async () => {
				ConnectorRewired.__Rewire__('axios', {
					get: sinon.stub().returns(Promise.reject({
						response: {
							status: 404,
							data: 'error'
						}
					}))
				});

				await returnedObject.onPackageCostFetchRequest();
				expect(dispatchSpy).to.be.calledWith({
					type: 'PACKAGE_COST_FETCH_FAILED',
					error: 'error'
				});
			});

			it('should dispatch PACKAGE_COST_FETCH_FAILED when package does not exists', async () => {
				ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({response: {status: 500}}))});

				await returnedObject.onPackageCostFetchRequest();
				expect(dispatchSpy).to.be.calledWith({
					type: 'PACKAGE_COST_FETCH_FAILED',
					error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
				});
			});

			it('should dispatch PACKAGE_COST_FETCH_FAILED when package does not exists', async () => {
				ConnectorRewired.__Rewire__('axios', {
					get: sinon.stub().returns(Promise.reject({
						response: {
							status: 400,
							data: {message: 'error'}
						}
					}))
				});

				await returnedObject.onPackageCostFetchRequest();
				expect(dispatchSpy).to.be.calledWith({
					type: 'PACKAGE_COST_FETCH_FAILED',
					error: 'error'
				});
			});
		});
		describe('onPackageDestroy', () => {
			it('should dispatch PACKAGE_DESTROY', async () => {
				const dispatchSpy = sinon.spy();
				const props = {params: {packageCode: 'PACKAGE0001'}};
				const returnedObject = mapDispatchToProps(dispatchSpy, props);
				await returnedObject.onPackageDestroy();
				expect(dispatchSpy).to.be.calledWith({
					type: 'PACKAGE_DESTROY'
				});
			});
		});

	});
});
