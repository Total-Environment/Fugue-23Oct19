import React from 'react';
import chai, {expect} from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import sinonChai from 'sinon-chai';
import sinon from 'sinon';
import { CreateExchangeRateActual, __RewireAPI__ as CreateExchangeRateRewired } from '../../../src/components/create-exchange-rate'

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('CreateExchangeRate', () => {
  describe('render', () => {
    let props,wrapper,onAddExchangeRateErrorCloseSpy,onAddExchangeRateSpy;
    beforeEach(() => {
      onAddExchangeRateErrorCloseSpy = sinon.spy();
      onAddExchangeRateSpy = sinon.spy();
      props = {
        "addExchangeRateError":false,
        "onAddExchangeRateErrorClose":onAddExchangeRateErrorCloseSpy,
        "onAddExchangeRate":onAddExchangeRateSpy,
        "exchangeRateAdding":false,
        currencyData: {values: {values: []},isFetched: true},
      };
      wrapper = shallow(<CreateExchangeRateActual {...props}/>);
    });

    it('should render Collapse', () => {
      expect(wrapper).to.have.exactly(1).descendants('Collapse');
    });
    //
    // it('should render Add New button', () => {
    //   const collapsePanel = wrapper.find('Collapse','Collapse.Panel').at(0);
    //   expect(collapsePanel).to.have.exactly(1).descendants('button');
    // });
  });
});
