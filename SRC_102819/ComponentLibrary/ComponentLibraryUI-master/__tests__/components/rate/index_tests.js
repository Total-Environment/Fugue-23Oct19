import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, {expect} from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { Rate, __RewireAPI__ as RateRewired } from '../../../src/components/rate'


chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('rate',()=>{
  describe('render', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        rates: [{
          "controlBaseRate": {
            "value": 10.0,
            "currency": "INR"
          },
          "landedRate": {
            "value": 35.700,
            "currency": "INR"
          },
          "insuranceCharges" : 10.0,
          "freightCharges" : 2.0,
          "basicCustomsDuty" : 5.0,
          "clearanceCharges" : 5.0,
          "taxVariance" : 0.03,
          "loationVariance" : 3.0,
          "marketFluctuation" : 0.04,
          "location": "Hyderabad",
          "typeOfPurchase":"DOMESTIC INTER-STATE",
          "id": "IRN000002",
          "appliedOn": "2017-01-30T00:00:00Z"
        }]
      };
      wrapper = shallow(<rate {...props}/>);
    });

    it('should return a div with proper class when rates are exist', () => {
      RateRewired.__Rewire__('styles', {rate: 'rate'});
      wrapper = shallow(<Rate {...props}/>);
      expect(wrapper.prop('className')).to.equal('rate');
    });

    it('should render rate when rates are present', () => {
      RateRewired.__Rewire__('styles', {rate: 'rate'});
      wrapper = shallow(<Rate {...props}/>);
      expect(wrapper).to.include.have.text('Hyderabad');
    });

    it('should render error message when rates are not present', () => {
      props.rates = [];
      RateRewired.__Rewire__('styles', {rate: 'rate'});
      wrapper = shallow(<Rate {...props}/>);
      expect(wrapper).to.have.exactly(1).descendants('h3');
    });

    it('should render error message when props have a message', () => {
      props = {rates:{message: 'Exchange Rate is invalid'}};
      wrapper = shallow(<Rate {...props}/>);
      expect(wrapper).to.have.descendants('h3');
      expect(wrapper.find('h3')).to.have.text('Exchange Rate is invalid');
    });
  });
});
