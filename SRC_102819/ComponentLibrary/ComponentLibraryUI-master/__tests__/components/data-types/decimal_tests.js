import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {Decimal} from '../../../src/components/data-types/Decimal';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe('Decimal', () => {
  let props, wrapper;

  beforeEach(() => {
    props = {
      columnName: "Diameter",
      columnValue: {
        "value": 25,
        "dataType": {
          "name": "Decimal",
          "subType": ""
        },
        validity: {isValid: true, msg: "Only numbers are allowed"},
      },
    };
    wrapper = shallow(<Decimal {...props}/>);
  });

  it('should render columnValue', () => {
    expect(wrapper).to.include.text('25');
  });

  it('should render input when editable is true', () => {
    props.columnValue.editable = true;
    wrapper = shallow(<Decimal {...props}/>);
    expect(wrapper).to.have.descendants('InputNumber');
    expect(wrapper.find('InputNumber')).to.have.prop('allowDecimals').to.equal(true);
    expect(wrapper.find('InputNumber')).to.have.prop('value').to.equal(25);
  });

  it('should render input when data is null and editable is true', () => {
    props.columnValue.editable = true;
    props.columnValue.value = null;
    wrapper = shallow(<Decimal {...props}/>);
    expect(wrapper).to.have.descendants('InputNumber');
    expect(wrapper.find('InputNumber')).to.have.prop('value').to.equal('');
  });

  describe('onChange', () => {
    beforeEach(() => {
      props.columnValue.editable = true;
      props.columnValue.validity = {isValid: false, msg: "Only numbers are allowed."},
        props.onChange = sinon.spy();
      wrapper = shallow(<Decimal {...props}/>);
    });
    it('should call onchange event with proper data', () => {
      expect(wrapper).to.have.descendants('InputNumber');
      wrapper.find('InputNumber').simulate('change', '1');
      expect(props.onChange).to.have.been.calledWith('1');
    });
    it('should call onchange event with null if data is null', () => {
      expect(wrapper).to.have.descendants('InputNumber');
      wrapper.find('InputNumber').simulate('change', null);
      expect(props.onChange).to.have.been.calledWith(null);
    });
  });
});
