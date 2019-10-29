import React from 'react';
import {shallow} from 'enzyme';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import {MoneyConnector} from '../../../src/components/data-types/money';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import configureStore from 'redux-mock-store';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Money', () => {
  it('should render column value as text', () => {
    const props = {
      columnName: "Last Purchase Rate", columnValue: {
        value: {
          amount: 127,
          currency: 'INR'
        }
      }
    };

    const mockStore = configureStore();
    const dispatch = sinon.spy();

    const  wrapper = shallow(<MoneyConnector {...props} dispatch={dispatch} store={mockStore({ reducer: {masterDataByName: {currency: {values: {values: ["INR","Euro"]}}}} })}/>);
    expect(wrapper.dive()).to.include.text('127 INR');
  });

  context('editable is true', () => {
    let props, wrapper;
    let mockStore;
    let dispatch;
    beforeEach(() => {
      props = {
        columnName: "Last Purchase Rate",
        columnValue: {
          value: {
            amount: 127,
            currency: 'INR'
          },
          editable: true,
          validity: {isValid: true, msg: ''}
        },
        onChange: sinon.spy()
      };

      mockStore = configureStore();
      dispatch = sinon.spy();

      wrapper = shallow(<MoneyConnector {...props} dispatch={dispatch} store={mockStore({ reducer: {masterDataByName: {currency: {values: {values: ["INR","Euro"]}}}} })}/>);
    });

    it('should render inputs with empty values when value is falsey', () => {
      props.columnValue.value = '';
      wrapper = shallow(<MoneyConnector {...props} dispatch={dispatch} store={mockStore({ reducer: {masterDataByName: {currency: {values: {values: ["INR","Euro"]}}}} })}/>);
      expect(wrapper.dive()).to.have.descendants('InputNumber');
      expect(wrapper.dive().find('InputNumber')).prop('value').to.equal('')

      expect(wrapper.dive()).to.have.descendants('span');
      expect(wrapper.dive().find('select')).prop('value').to.equal('');
    });

    it('should render inputs', () => {
      expect(wrapper.dive().find('InputNumber')).prop('value').to.equal(127);
      expect(wrapper.dive().find('select')).prop('value').to.equal('INR');
    });

    it('should call onChange with new amount when first input is changed', () => {
      wrapper.dive().find('InputNumber').simulate('change', '128');
      expect(props.onChange).to.have.been.calledWith({amount: '128', currency: 'INR'});
    });

    it('should call onChange with null when first input value is null', () => {
      wrapper.dive().find('InputNumber').simulate('change', '');
      expect(props.onChange).to.have.been.calledWith(null);
    });

    it('should call onChange with new amount when second input is changed', () => {
      wrapper.dive().find('select').simulate('change', {target: {value: 'USD'}});
      expect(props.onChange).to.have.been.calledWith({amount: 127, currency: 'USD'});
    });

    it('should --select-- in currency when value is null', () => {
      props.columnValue.value = null;
      wrapper = shallow(<MoneyConnector {...props} dispatch={dispatch} store={mockStore({ reducer: {masterDataByName: {currency: {values: {values: ["INR","Euro"]}}}} })}/>);
      expect(wrapper.dive().find('select')).to.have.prop('value').to.be.empty;
    });
  });
});
