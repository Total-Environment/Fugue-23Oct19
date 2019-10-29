import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps,__RewireAPI__ as ConnectorRewired} from '../src/exchange_rate_history_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('ExchangeRateHistoryConnector', () => {
  describe('mapStateToProps', () => {
    it('should returns exchange rate history', () => {
      const date = new Date();
      const state = {reducer:{exchangeRateHistory: [{exchangeRate: {value: 70}}], error: '', masterDataByName: {currency:{values: {values: ['USD', 'INR']}}}}};
      const props = {location: {query: {pageNumber: 1, sortColumn: 'AppliedFrom', sortOrder: 'Descending', currencyType: 'USD', appliedFrom: date}}};
      const expectedOutput = mapStateToProps(state, props);
      expect(expectedOutput).to.deep.equal({
        exchangeRateHistory: [{exchangeRate: {value: 70}}],
        error: state.reducer.error,
        addExchangeRateError:undefined,
        exchangeRateAdding:false,
        pageNumber: 1,
        sortColumn: 'AppliedFrom',
        sortOrder: 'Descending',
        currencyType: 'USD',
        appliedFrom: date,
        currencyData: {values: {values: ['USD', 'INR']}}
      });
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onExchangeRateHistoryFetchRequest', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch EXCHANGE_RATE_HISTORY_FETCH_SUCCEEDED when exchange rate history exists', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data:'exchangeRateHistoryDetails'}))});
        await returnedObject.onExchangeRateHistoryFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'EXCHANGE_RATE_HISTORY_FETCH_SUCCEEDED',
          details: 'exchangeRateHistoryDetails'
        });
      });

      it('should dispatch EXCHANGE_RATE_HISTORY_FETCH_FAILED when response status is 500', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({error: 'error', message: 'error has occurred'}))});
        await returnedObject.onExchangeRateHistoryFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'EXCHANGE_RATE_HISTORY_FETCH_FAILED',
          error: 'error has occurred'
        });
      });
    });

    describe('onAddExchangeRate', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      // it('should dispatch ADD_EXCHANGE_RATE_SUCCEEDED when exchange rate added', async() => {
      //   ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.resolve())});
      //   let exchangeRate = {appliedFrom:'01/01/2017'};
      //   await returnedObject.onAddExchangeRate(exchangeRate);
      //   expect(dispatchSpy).to.be.calledWith({
      //     type: 'ADD_EXCHANGE_RATE_SUCCEEDED'
      //   });
      // });

      it('should dispatch ADD_EXCHANGE_RATE_FAILED when response status is 409', async() => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response:{status:409} }))});
        let exchangeRate = {appliedFrom:'01/01/2017', fromCurrency:'USD'};
        await returnedObject.onAddExchangeRate(exchangeRate);
        const FormattedDate = new Date(exchangeRate.appliedFrom);
        const FormattedDateString = FormattedDate.getDate()+"/"+(FormattedDate.getMonth() + 1).toString()+"/"+FormattedDate.getFullYear();
        expect(dispatchSpy).to.be.calledWith({
          type: 'ADD_EXCHANGE_RATE_FAILED',
          error: 'Duplicate entries are not allowed. A exchange rate version for \'' + exchangeRate.fromCurrency + '\' on \''+FormattedDateString+'\' already exists'
        });
      });

      it('should dispatch ADD_EXCHANGE_RATE_FAILED when response status is 500', async() => {
        ConnectorRewired.__Rewire__('axios', {post: sinon.stub().returns(Promise.reject({response:{status:500, data:{message: 'error is found'}},message:"error " }))});
        let exchangeRate = {appliedFrom:'01/01/2017'};
        await returnedObject.onAddExchangeRate(exchangeRate);
        expect(dispatchSpy).to.be.calledWith({
          type: 'ADD_EXCHANGE_RATE_FAILED',
          error: 'error is found'
        });
      });
    });

    describe('onAddExchangeRateErrorClose',()=>{
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch ADD_EXCHANGE_RATE_ERROR_CLOSED', async() => {
        await returnedObject.onAddExchangeRateErrorClose();
        expect(dispatchSpy).to.be.calledWith({
          type: 'ADD_EXCHANGE_RATE_ERROR_CLOSED'
        });
      });
    });

    describe('onDestroyExchangeRateHistory',()=>{
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch DESTROY_EXCHANGE_RATE_HISTORY', async() => {
        await returnedObject.onDestroyExchangeRateHistory();
        expect(dispatchSpy).to.be.calledWith({
          type: 'DESTROY_EXCHANGE_RATE_HISTORY'
        });
      });
    });
  });
});

