import sinon from 'sinon';
import {mapStateToProps, mapDispatchToProps,__RewireAPI__ as ConnectorRewired} from '../src/exchange_rate_connector';
import chai, {expect} from 'chai';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);

describe('ExchangeRateConnector', () => {
  describe('mapStateToProps', () => {
    it('should returns exchange rates when passed props with state', () => {
      const state = {reducer:{currentExchangeRates: [{exchangeRate: {value: 70}}], exchangeRatesError: ''}};
      const props = {};
      const expectedOutput = mapStateToProps(state, props);
      expect(expectedOutput).to.deep.equal({
        exchangeRates: [{exchangeRate: {value: 70}}],
        exchangeRatesError: ''
      });
    });
  });

  describe('mapDispatchToProps', () => {
    describe('onExchangeRatesFetchRequest', () => {
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch EXCHANGE_RATES_FETCH_SUCCEEDED when exchange rates exist', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.resolve({data:'exchangeRates'}))});
        await returnedObject.onExchangeRatesFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'EXCHANGE_RATES_FETCH_SUCCEEDED',
          exchangeRates: 'exchangeRates'
        });
      });

      it('should dispatch EXCHANGE_RATES_FETCH_FAILED when response status is 500', async() => {
        ConnectorRewired.__Rewire__('axios', {get: sinon.stub().returns(Promise.reject({error: 'error', message: 'error has occurred'}))});
        await returnedObject.onExchangeRatesFetchRequest();
        expect(dispatchSpy).to.be.calledWith({
          type: 'EXCHANGE_RATES_FETCH_FAILED',
          exchangeRatesError: 'error has occurred'
        });
      });
    });

    describe('onExchangeRatesDestroy',()=>{
      let dispatchSpy, returnedObject;
      beforeEach(() => {
        dispatchSpy = sinon.spy();
        returnedObject = mapDispatchToProps(dispatchSpy);
      });

      it('should dispatch EXCHANGE_RATES_DESTROYED', async() => {
        await returnedObject.onExchangeRatesDestroy();
        expect(dispatchSpy).to.be.calledWith({
          type: 'EXCHANGE_RATES_DESTROYED'
        });
      });
    });
  });
});

