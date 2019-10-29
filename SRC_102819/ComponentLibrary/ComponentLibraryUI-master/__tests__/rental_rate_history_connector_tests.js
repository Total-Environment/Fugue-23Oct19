import sinon from 'sinon';
import {
  mapStateToProps,
  mapDispatchToProps,
  __RewireAPI__ as ConnectorRewired
} from '../src/rental_rate_history_connector';
import chai, {expect} from 'chai';
import {apiUrl} from '../src/helpers';
import sinonChai from 'sinon-chai';
import 'babel-polyfill';

chai.use(sinonChai);

describe('RentalRateHistoryConnector', () => {
  describe('mapStateToProps', () => {
    it('should returns rental rate history details when passed props with material code and state', () => {
      const state = {
        reducer: {
          rentalRateHistory:{'MTRL00001':"Rental Rates"},
          addRentalRateError: "Error",
          rentalRateAdding: true,
          masterDataByName: {rental_unit: 1, currency: 2},
        },
        error: ''
      };
      const props = {
        params: {materialCode: 'MTRL00001'},
        location: {query: {pageNumber: 1, sortOrder: 'Ascending', sortColumn: 'Value'}}
      };
      const expectedOutput = mapStateToProps(state, props);
      expect(expectedOutput).to.deep.equal({
        materialCode: 'MTRL00001',
        rentalRateHistory: "Rental Rates",
        error: state.reducer.error,
        addRentalRateError: "Error",
        rentalRateAdding: true,
        sortOrder: 'Ascending',
        sortColumn: 'Value',
        appliedFrom: undefined,
        rentalUnit: undefined,
        rentalUnitData: 1,
        currencyData: 2,
      });
    });
  });

  describe('mapDispatchToProps', () => {
    let ownProps;
    beforeEach(() => {
      ownProps = {
        params: {
          materialCode: 'MTRL00001',
        },
        location: {query: {
          sortOrder: 'Ascending',
          sortColumn: 'Value',
        }}
      };
    });
    describe('onRentalRateHistoryFetchRequest', () => {

      it('should dispatch RENTAL_RATE_HISTORY_FETCH_SUCCEEDED for material when rental rate history exists', async () => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data: 'rentalRateHistoryDetails'}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onRentalRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATE_HISTORY_FETCH_SUCCEEDED',
          rentalRateHistory: {code:'MTRL00001',data:'rentalRateHistoryDetails'}
        });
      });

      it('should dispatch RENTAL_RATE_HISTORY_FETCH_FAILED when response status is 500', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 500}}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onRentalRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',
          materialCode:'MTRL00001',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch RENTAL_RATE_HISTORY_FETCH_FAILED when response status is 403', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 403}}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onRentalRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',
          materialCode:'MTRL00001',
          error: 'Some thing went wrong. Problem has been recorded and will be fixed.'
        });
      });

      it('should dispatch RENTAL_RATE_HISTORY_FETCH_FAILED when response status is 404', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({response: {status: 404}}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onRentalRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',
          materialCode:'MTRL00001',
          error: 'No rental rate history is found.'
        });
      });

      it('should dispatch RENTAL_RATE_HISTORY_FETCH_FAILED when response status is undefined', async () => {
        ConnectorRewired.__Rewire__('axios', {
          get: sinon.stub()
            .returns(Promise.reject({message: "Error"}))
        });
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onRentalRateHistoryFetchRequest('MTRL00001');
        expect(dispatchSpy).to.be.calledWith({
          type: 'RENTAL_RATE_HISTORY_FETCH_FAILED',
          materialCode:'MTRL00001',
          error: 'Error'
        });
      });
    });
    describe('onAddRentalRate', () => {
      it('should call dispatch with ADD_RENTAL_RATE_REQUESTED and ADD_RENTAL_RATE_SUCCEEDED', async () => {
        let postStub = sinon.stub();
        ConnectorRewired.__Rewire__('axios', {post: postStub.withArgs(apiUrl('/material-rates/MTRL00001/rental-rates'), {}).returns(Promise.resolve({data: {id: 1}}))});
        sinon.stub(window.location, 'reload');
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onAddRentalRate('MTRL00001', {});
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RENTAL_RATE_REQUESTED'});
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RENTAL_RATE_SUCCEEDED'});
        window.location.reload.restore();
      });

      it('should call dispatch with ADD_RENTAL_RATE_FAILED', async () => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().withArgs(apiUrl('/material-rates/MTRL00001/rental-rates'), {}).returns(Promise.reject({response: {data: {message: "error"}}}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onAddRentalRate('MTRL00001', {});
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RENTAL_RATE_FAILED', error: "error"});
      });

      it('should call dispatch with ADD_RENTAL_RATE_FAILED when response code is 409', async () => {
        const rate = {appliedFrom: '2013-03-10T02:00:00Z', unitOfMeasure: 'Daily'};
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().withArgs(apiUrl('/material-rates/MTRL00001/rental-rates'), rate).returns(Promise.reject({response: {status: 409}}))});
        const dispatchSpy = sinon.spy();
        await mapDispatchToProps(dispatchSpy, ownProps).onAddRentalRate('MTRL00001', rate);
        expect(dispatchSpy).to.be.calledWith({
          type: 'ADD_RENTAL_RATE_FAILED',
          error: "Duplicate entries are not allowed. A rental rate version on '10/3/2013' for 'Daily' already exists"
        });
      });
    });

    describe('onAddRentalRateErrorClose', () => {
      it('should call dispatch with ADD_RENTAL_RATE_ERROR_CLOSED', () => {
        const dispatchSpy = sinon.spy();
        mapDispatchToProps(dispatchSpy, ownProps).onAddRentalRateErrorClose();
        expect(dispatchSpy).to.be.calledWith({type: 'ADD_RENTAL_RATE_ERROR_CLOSED'});
      });
    });
  });
});
