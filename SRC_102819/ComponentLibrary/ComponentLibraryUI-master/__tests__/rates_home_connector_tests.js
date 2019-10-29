import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/rates_home_connector';
import chai, {expect} from 'chai';
import {apiUrl} from '../src/helpers';
import sinonChai from 'sinon-chai';
import 'babel-polyfill';

chai.use(sinonChai);

describe('RatesHomeConnector', () => {
  describe('mapStateToProps', () => {
    it('should return rates details when passed props with state', () => {
      const state = {
        reducer: {
          rates: {material: [{rate: {value: 70}, "id": "MTRL00001"}], ratesError: ''},
          rateFilters: {material: 1},
          fetchingMasterData: false,
          ratesEditable: {material: false},
          bulkRateError: {},
          masterDataByName: {currency: {values: ['INR','USD']}, type_of_purchase: {values: ["Import", "Domestic Intrastate"]}}
        }
      };
      const props = {
        location: {query: {pageNumber: 1, sortColumn: 'Applied On', sortOrder: 'Ascending'}},
        route: {componentType: 'material'}
      };
      const actual = mapStateToProps(state, props);
      expect(actual).to.deep.equal({
        classificationData: undefined,
        isFilterApplied: false,
        filters: 1,
        isFetchingRates: false,
        masterData: {currency: {values: ['INR','USD']}, type_of_purchase: {values: ["Import", "Domestic Intrastate"]}},
        masterDataError: undefined,
        rates: [{rate: {value: 70}, "id": "MTRL00001"}],
        ratesError: state.reducer.ratesError,
        componentType: 'material',
        fetchingMasterData: false,
        bulkRateError: undefined,
        editable: false,
        currencyData: {values: ['INR','USD']},
        typeOfPurchaseData: {values: ["Import", "Domestic Intrastate"]},
        typeOfPurchaseError: undefined,
        locationError: undefined,
        currencyError: undefined,
        statusError: undefined
        });
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onRatesFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy, {route: {componentType: 'material'}});
      });

      it('should dispatch RATES_FETCH_SUCCEEDED when rates exists', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.resolve({data: 'ratesDetails'}))});
        await returnedObject.onRatesFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          componentType: 'material',
          type: 'RATES_FETCH_SUCCEEDED',
          rates: 'ratesDetails'
        });
      });
      it('should dispatch RATES_FETCH_FAILED when response status is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response: {data:{message: 'error has occurred'},status: 500}}))});
        await returnedObject.onRatesFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'RATES_FETCH_FAILED',
          ratesError: 'error has occurred',
          componentType: 'material'
        });
      });
    });
    describe('onDestroyRates', () => {
      it('should call dispatch with DESTROY_RATES', () => {
        const dispatchSpy = sinon.spy();
        mapDispatchToProps(dispatchSpy, {route: {componentType: 'material'}}).onDestroyRates();
        expect(dispatchSpy).to.be.calledWith({type: 'DESTROY_RATES', componentType: 'material'});
      });
    });
  });


});
