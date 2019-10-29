import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { CreateBrand, __RewireAPI__ as CreateBrandRewired } from '../../../src/components/create-brand';

chai.should();
chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('Create Brand', () => {
  let props, onAddBrandSpy, onCancelSpy;
  beforeEach(() => {
    onAddBrandSpy = sinon.spy();
    onCancelSpy = sinon.spy();
    props = {
      brandDefinition: {
        columns: [
          {
            "dataType": {
              "name": "String",
              "subType": null
            },
            "isRequired": true,
            "isSearchable": true,
            "key": "manufacturer's_name",
            "name": "Manufacturer's Name"
          }
        ]
      },
      isAddingBrand: false,
      onAddBrand: onAddBrandSpy,
      onCancel: onCancelSpy
    };
  });

  describe('render', () => {
    it('should render', () => {
      const wrapper = shallow(<CreateBrand {...props} />);
      expect(wrapper).to.have.descendants('div');
      expect(wrapper).to.have.descendants('DataType');
      expect(wrapper).to.have.descendants('AlertDialog');
      expect(wrapper).to.have.descendants('ConfirmationDialog');
      expect(wrapper.find('input').at(0).props().value).to.equal('Add');
      expect(wrapper.find('a').at(0)).to.have.text('Cancel');
    });

    it('should render ConfirmationDialog when Cancel button is clicked', () => {
      const wrapper = shallow(<CreateBrand {...props} />);
      wrapper.find('a').at(0).simulate('click');
      expect(wrapper).to.have.descendants('ConfirmationDialog');
      expect(wrapper.find('ConfirmationDialog')).to.have.prop('message').to.equal('The brand will not be created. Do you wish to continue?');
      expect(wrapper.find('ConfirmationDialog')).to.have.prop('shown').to.be.true;
    });

    it('should call onCancel when cancel button is clicked and yes button is clicked on ConfirmationDialog', () => {
      const wrapper = shallow(<CreateBrand {...props} />);
      wrapper.find('a').simulate('click');
      expect(wrapper).to.have.descendants('ConfirmationDialog');
      wrapper.find('ConfirmationDialog').simulate('yes');
      expect(wrapper.find('ConfirmationDialog')).to.have.prop('shown').to.be.false;
      props.onCancel.should.have.been.called;
    });

    it('should not call onCancel when cancel button is clicked and no button is clicked on ConfirmationDialog', () => {
      const wrapper = shallow(<CreateBrand {...props} />);
      wrapper.find('a').simulate('click');
      expect(wrapper).to.have.descendants('ConfirmationDialog');
      wrapper.find('ConfirmationDialog').simulate('no');
      expect(wrapper.find('ConfirmationDialog')).to.have.prop('shown').to.be.false;
      expect(props.onCancel).to.be.not.called;
    });

    it('should call onAddBrand when form is submitted', () => {
      const wrapper = shallow(<CreateBrand {...props} />);
      const preventDefaultSpy = sinon.spy();
      const expected = [
        {
          dataType: {
            name: 'String',
            subType: null
          },
          isRequired: true,
          isSearchable: true,
          editable: true,
          key: "manufacturer's_name",
          name: "Manufacturer's Name",
          value: 'Test',
          validity: { isValid: true, msg: '' }
        }
      ];
      wrapper.find('DataType').simulate('change', 'Test');
      wrapper.find('#add-button').simulate('click', { preventDefault: preventDefaultSpy });
      props.onAddBrand.should.have.been.calledWith(expected);
    });
  });
});
