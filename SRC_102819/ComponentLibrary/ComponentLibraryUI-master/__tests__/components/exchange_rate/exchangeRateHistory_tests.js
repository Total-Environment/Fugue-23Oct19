import React from 'react';
import chai, {expect} from 'chai';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import {
  ExchangeRateHistory,
  __RewireAPI__ as ExchangeRateHistoryRewired
} from '../../../src/components/exchange-rate-history/index'
import {permissions} from "../../../src/permissions/permissions";

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('ExchangeRateHistory', () => {
  describe('ComponentDidMount', () => {
    let onExchangeRateHistoryFetchRequestSpy, props, wrapper;
    beforeEach(() => {
      onExchangeRateHistoryFetchRequestSpy = sinon.spy();
      props = {onExchangeRateHistoryFetchRequest: onExchangeRateHistoryFetchRequestSpy, error: '', fetchMasterDataByNameIfNeeded: sinon.spy()};
      wrapper = shallow(<ExchangeRateHistory {...props}/>);
    });

    it('should call onExchangeRateHistoryFetchRequest when it does not have exchange rate history', () => {
      wrapper.instance().componentDidMount();
      expect(onExchangeRateHistoryFetchRequestSpy).to.be.calledWith();
    });
  });
  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        exchangeRateHistory: {
          items: [{
            "fromCurrency": "USD",
            "baseConversionRate": "60",
            "currencyFluctuationCoefficient": "0.25",
            "definedConversionRate": "75",
            "appliedFrom": "2016-12-13T00:00:00Z"
          }],
          pageNumber: 1,
          sortColumn: 'AppliedFrom',
          sortOrder: 'Descending',
        },
        currencyData: {values: {values: []},isFetched: true},
        error: '',
      };
      wrapper = shallow(<ExchangeRateHistory {...props}/>);
    });

    it('should render table', () => {
      const tableHeaderNode = wrapper.find('Table');
      const expectedHeaders = [{
        "name": "Currency",
        "key": "fromCurrency",
        "sortKey": "FromCurrency",
        "sortable": true
      }, {
        "name": "Conversion Rate",
        "key": "baseConversionRate",
        "type": "number",
        "sortKey": "BaseConversionRate",
        "sortable": false
      }, {
        "name": "Currency Fluctuation Coefficient (%)",
        "key": "currencyFluctuationCoefficient",
        "type": "number",
        "sortKey": "FluctuationCoefficient",
        "sortable": false
      }, {
        "name": "Defined Conversion Rate",
        "key": "definedConversionRate",
        "type": "number",
        "sortKey": "DefinedConversionRate",
        "sortable": false
      }, {"name": "Applied From", "key": "appliedFrom", "type": "number", "sortKey": "AppliedFrom", "sortable": true}];
      expect(tableHeaderNode.prop('headers')).to.deep.equal(expectedHeaders);
      expect(tableHeaderNode.prop('data'));
    });

    it('should render all values of exchange rates in proper order', () => {
    });

    // it('should render CreateExchangeRate', () => {
    //   expect(wrapper).to.have.exactly(1).descendants('CreateExchangeRate');
    // });

    it('should render error message when exchange rates history not present', () => {
      props.exchangeRateHistory = [];
      ExchangeRateHistoryRewired.__Rewire__('styles', {exchangeRateHistory: 'exchangeRateHistory'});
      wrapper = shallow(<ExchangeRateHistory {...props}/>);
      expect(wrapper).to.have.exactly(1).descendants('h3');
      expect(wrapper.find('h3')).to.have.text('No exchange rate history found.');
    });

    it('should render error message when props have a message', () => {
      props = {error: 'Some Error Occured'};
      wrapper = shallow(<ExchangeRateHistory {...props}/>);
      expect(wrapper).to.have.descendants('h3');
      expect(wrapper.find('h3')).to.have.text('Some Error Occured');
    });
  });
});
