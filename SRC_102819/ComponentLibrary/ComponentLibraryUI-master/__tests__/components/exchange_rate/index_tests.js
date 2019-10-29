import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { ExchangeRate, __RewireAPI__ as ExchangeRateRewired } from '../../../src/components/exchange-rate'


chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('exchangeRate',()=>{
  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        exchangeRates: [{
          "fromCurrency":"USD",
          "baseConversionRate":"60",
          "currencyFluctuationCoefficient":"0.25",
          "definedConversionRate":"75",
          "appliedFrom": "2016-12-13T00:00:00Z"
        }]
      };
      wrapper = shallow(<exchangeRate {...props}/>);
    });

    it('should return a div with proper class when exchange rates exist', () => {
      ExchangeRateRewired.__Rewire__('styles', {exchangeRate: 'exchangeRate'});
      wrapper = shallow(<ExchangeRate {...props}/>);
      expect(wrapper.prop('className')).to.equal('exchangeRate');
    });

    it('should render exchange rate when exchange rates are present', () => {
      ExchangeRateRewired.__Rewire__('styles', {exchangeRate: 'exchangeRate'});
      wrapper = shallow(<ExchangeRate {...props}/>);
      expect(wrapper).to.include.have.text('USD');
    });

    it('should render error message when exchange rates are not present', () => {
      props.exchangeRates = [];
      ExchangeRateRewired.__Rewire__('styles', {exchangeRate: 'exchangeRate'});
      wrapper = shallow(<ExchangeRate {...props}/>);
      expect(wrapper).to.have.exactly(1).descendants('h3');
      expect(wrapper.find('h3')).to.have.text('No current exchange rates are found.');
    });

    it('should render error message when props have a message', () => {
      props = {exchangeRatesError: 'Some Error Occured'};
      wrapper = shallow(<ExchangeRate {...props}/>);
      expect(wrapper).to.have.descendants('h3');
      expect(wrapper.find('h3')).to.have.text('Some Error Occured');
    });
  });
});
