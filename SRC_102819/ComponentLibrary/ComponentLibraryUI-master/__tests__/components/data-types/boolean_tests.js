import {shallow} from 'enzyme';
import chai, {expect} from 'chai';
import React from 'react';
import chaiEnzyme from 'chai-enzyme';
import {Boolean} from '../../../src/components/data-types/boolean';
import {idFor} from '../../../src/helpers';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Boolean', () => {
  it('should render Yes when value is true', () => {
    const props = {
      columnName: "short Description", columnValue: {
        "value": true,
        "dataType": {
          "name": "Boolean",
          "subType": null
        }
      }
    };
    const wrapper = shallow(<Boolean {...props}/>);
    expect(wrapper).to.have.text('Yes');
  });

  it('should render No when value is False', () => {
    const props = {
      columnName: "short Description", columnValue: {
        "value": false,
        "dataType": {
          "name": "Boolean",
          "subType": null
        }
      }
    };
    const wrapper = shallow(<Boolean {...props}/>);
    expect(wrapper).to.have.text('No');
  });

  context('when it is editable', () => {
    let props, wrapper;
    beforeEach(() => {
      props = {
        columnName: "short Description",
        columnValue: {
          "value": true,
          "dataType": {
            "name": "Boolean",
            "subType": null
          },
          editable: true
        },
        onChange: sinon.spy()
      };
      wrapper = shallow(<Boolean {...props}/>);
    });
    it('should render a input as radio button', () => {
      expect(wrapper).to.have.descendants('input');
      expect(wrapper.find('input').first()).to.have.prop('name').deep.equal(idFor('short Description', 'radio'));
    });

    it('should render first input as checked when it is true', () => {
      expect(wrapper.find('input').first()).to.have.prop('checked').equal(true);
      expect(wrapper.find('input').at(1)).to.have.prop('checked').equal(false);
    });

    it('should render second input as checked when it is false', () => {
      props.columnValue.value = false;
      wrapper = shallow(<Boolean {...props}/>);
      expect(wrapper.find('input').first()).to.have.prop('checked').equal(false);
      expect(wrapper.find('input').at(1)).to.have.prop('checked').equal(true);
    });

    it('should not render second input as checked when it is null', () => {
      props.columnValue.value = null;
      wrapper = shallow(<Boolean {...props}/>);
      expect(wrapper.find('input').first()).to.have.prop('checked').equal(false);
      expect(wrapper.find('input').at(1)).to.have.prop('checked').equal(false);
    });

    it('should call onChange with true when first input is checked', () => {
      wrapper.find('input').first().simulate('change');
      expect(props.onChange).to.be.calledWith(true);
    });
    it('should call onChange with false when first input is checked', () => {
      wrapper.find('input').at(1).simulate('change');
      expect(props.onChange).to.be.calledWith(false);
    });
  });
});
