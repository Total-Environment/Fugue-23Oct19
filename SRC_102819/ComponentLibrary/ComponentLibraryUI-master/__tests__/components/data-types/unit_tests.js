import chai, {expect} from 'chai';
import React from 'react';
import {shallow} from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import {Unit} from '../../../src/components/data-types/unit';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(sinonChai);
chai.use(chaiEnzyme());

describe('Unit', () => {
  let props, wrapper;

  beforeEach(() => {
    props = {
      columnName: "Diameter",
      columnValue: {
        "value": {
          "value": '25',
          "type": "mm"
        },
        "dataType": {
          "name": "Unit",
          "subType": "mm"
        },
        validity: {isValid: true, msg: "Only numbers are allowed"},
      },
    };
    wrapper = shallow(<Unit {...props}/>);
  });

  it('should render columnValue with subType if value is not null', () => {
    expect(wrapper).to.have.text('25 mm');
  });

  it('should render - if value is null', () => {
    props = {
      columnName: "Diameter", columnValue: {
        "value": null,
        "dataType": {
          "name": "Unit",
          "subType": "mm"
        },
        validity: {isValid: true, msg: "Only numbers are allowed"},
      },
    };
    wrapper = shallow(<Unit {...props}/>);
    expect(wrapper).to.have.text('-');
  });

  it('should render input when editable is true', () => {
    props.columnValue.editable = true;
    wrapper = shallow(<Unit {...props}/>);
    expect(wrapper).to.have.descendants('InputNumber');
    expect(wrapper.find('InputNumber')).to.have.prop('value').to.equal('25');
    expect(wrapper).to.include.text('mm');
  });

  it('should render input when data is null and editable is true', () => {
    props.columnValue.editable = true;
    props.columnValue.value = null;
    wrapper = shallow(<Unit {...props}/>);
    expect(wrapper).to.have.descendants('InputNumber');
    expect(wrapper.find('InputNumber')).to.have.prop('value').to.equal('');
    expect(wrapper).to.include.text('mm');
  });

  describe('onChange', () => {
    beforeEach(() => {
      props.columnValue.editable = true;
      props.columnValue.validity = {isValid: false, msg: "Only numbers are allowed."},
        props.onChange = sinon.spy();
      wrapper = shallow(<Unit {...props}/>);
    });
    it('should call onchange event with proper data', () => {
      expect(wrapper).to.have.descendants('InputNumber');
      wrapper.find('InputNumber').simulate('change', '1');
      expect(props.onChange).to.have.been.calledWith({value: '1', type: 'mm'});
    });
    it('should call onchange event with null if data is null', () => {
      expect(wrapper).to.have.descendants('InputNumber');
      wrapper.find('InputNumber').simulate('change', null);
      expect(props.onChange).to.have.been.calledWith(null);
    });
  });



  it('should render a input also when data is not valid, with invalid data', () => {
    props = {
      columnName: "Diameter", columnValue: {
        "value": {value: 3, type: 'mm'},
        "dataType": {
          "name": "Unit",
          "subType": "mm"
        },
        editable: true,
        validity: {isValid: true, msg: "Only numbers are allowed."},
      },
      onChange: sinon.spy(),
    };
    wrapper = shallow(<Unit {...props}/>);
    wrapper.find('InputNumber').simulate('change', 'a');
    expect(wrapper).to.have.exactly(1).descendants('InputNumber');
  });
});
