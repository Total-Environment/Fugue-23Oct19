import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps, __RewireAPI__ as ConnectorRewired} from '../src/rate_history_connector';
import chai, {expect} from 'chai';
import {apiUrl} from '../src/helpers';
import sinonChai from 'sinon-chai';
import 'babel-polyfill';

chai.use(sinonChai);

describe('RateHistoryConnector', () => {
  describe('mapStateToProps', () => {
    it('should returns rate history details when passed props with material code and state', () => {
      const state = {reducer: {rateHistory: [{rate: {value: 70}, "id": "MTRL00001"}], rateHistoryError: '', masterDataByName: {type_of_purchase: 1, location: 2, currency: 3}}};
      const date = new Date();
      const props = {
        params: {componentCode: 'MTRL00001'},
        route: {componentType: 'material'},
        location: {query: {pageNumber: 1, sortColumn: 'Material Name', sortOrder: 'Ascending', typeOfPurchase: 'P', location:'Hyd', appliedOn: date}}
      };
      const expectedOutput = mapStateToProps(state, props);
      expect(expectedOutput).to.deep.equal({
        componentCode: 'MTRL00001',
        componentType: 'material',
        rateHistory: [{rate: {value: 70}, "id": "MTRL00001"}],
        error: state.reducer.rateHistoryError,
        addRateError: undefined,
        rateAdding: false,
        pageNumber: 1,
        sortColumn: 'Material Name',
        sortOrder: 'Ascending',
        typeOfPurchase: 'P',
        location: 'Hyd',
        appliedOn: date,
        typeOfPurchaseData: 1,
        locationData: 2,
        currencyData: 3,
        typeOfPurchaseError: undefined,
        locationError: undefined,
        currencyError: undefined

      });
    });
    it('should set value of rateAdding if the value is passed', () => {
      const state = {reducer: {rateHistory: [{rate: {value: 70}, "id": "MTRL00001"}], error: '', rateAdding: true, masterDataByName: {type_of_purchase: 1, location: 2}}};
      const props = {
        params: {componentCode: 'MTRL00001'},
        route: {componentType: 'material'},
        location: {query: {pageNumber: 1, sortColumn: 'Material Name', sortOrder: 'Ascending'}}
      };
      const expected = mapStateToProps(state, props);
      expect(expected.rateAdding).to.be.equal(true);
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onRateHistoryFetchRequest', () => {
      let dispatchSpy, returnedObject, fetchStub;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch RATE_HISTORY_FETCH_SUCCEEDED for material when rate history exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'rateHistoryDetails'}))});
        await returnedObject.onRateHistoryFetchRequest('MTRL00001', 'material');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RATE_HISTORY_FETCH_SUCCEEDED',
          details: 'rateHistoryDetails'
        });
      });

      it('should dispatch RATE_HISTORY_FETCH_SUCCEEDED for service when rate history exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'rateHistoryDetails'}))});
        await returnedObject.onRateHistoryFetchRequest('FDP0001', 'service');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RATE_HISTORY_FETCH_SUCCEEDED',
          details: 'rateHistoryDetails'
        });
      });

      it('should dispatch RATE_HISTORY_FETCH_FAILED when response status is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 500}}))
        });
        await returnedObject.onRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RATE_HISTORY_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch RATE_HISTORY_FETCH_FAILED when response status is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 404}}))
        });
        await returnedObject.onRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RATE_HISTORY_FETCH_FAILED',
          error: 'No rate history is found'
        });
      });

      it('should dispatch RATE_HISTORY_FETCH_FAILED when response status is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 403}}))
        });
        await returnedObject.onRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RATE_HISTORY_FETCH_FAILED',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

    });
    describe('onAddRate', () => {

      it('should call dispatch with ADD_RATE_REQUESTED and ADD_RATE_SUCCEEDED', async () => {
        let postStub = sinon.stub();
        ConnectorRewired.__Rewire__('axios', {post: postStub.withArgs(apiUrl('/material-rates'), {}).returns(Promise.resolve({data: {id: 1}}))});
        sinon.stub(window.location, 'reload');
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddRate({}, 'material');
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RATE_REQUESTED'});
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RATE_SUCCEEDED'});
        window.location.reload.restore();
      });

      it('should call dispatch with ADD_RATE_FAILED', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().withArgs(apiUrl('/material-rates'), {}).returns(Promise.reject({response: {status: 500, data: {message: "error"}}}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddRate({}, 'material');
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RATE_FAILED', error: "error"});
      });

      it('should call dispatch with ADD_RATE_SUCCEEDED when error message contains error as exchange rate found', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().withArgs(apiUrl('/material-rates'), {}).returns(Promise.reject({response: {status: 400, data: {message: "No exchange rate is found for currency:"}}}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddRate({}, 'material');
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RATE_SUCCEEDED'});
      });

      it('should call dispatch with ADD_RATE_FAILED when response code is 409', async () => {
        const rate = {appliedOn: '2013-03-10T02:00:00Z', location: 'location', typeOfPurchase: 'import'};
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().withArgs(apiUrl('/material-rates'), rate).returns(Promise.reject({response: {status: 409}}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy).onAddRate(rate, 'material');
        expect(dispatchSpy).to.be.calledWith({
          type: 'ADD_RATE_FAILED',
          error: "Duplicate entries are not allowed. A rate version on '10/3/2013' for 'location' with mode of purchase as 'import' already exists"
        });
      });
    });
    describe('onAddRateErrorClose', () => {
      it('should call dispatch with ADD_RATE_ERROR_CLOSED', () => {
        const dispatchSpy = sinon.spy();
        mapDispatchToProps(dispatchSpy).onAddRateErrorClose();
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RATE_ERROR_CLOSED'});
      });
    });
    describe('onDestroyRateHistory', () => {
      it('should call dispatch with DESTROY_RATE_HISTORY', () => {
        const dispatchSpy = sinon.spy();
        mapDispatchToProps(dispatchSpy).onDestroyRateHistory();
        expect(dispatchSpy).to.be.calledWith({type: 'DESTROY_RATE_HISTORY'});
      });
    });
  });
});

