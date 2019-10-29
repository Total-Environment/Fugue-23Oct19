import React from 'react';
import {shallow} from 'enzyme';
import chai, {expect} from 'chai';
import {SortComponent} from '../../../src/components/sort-component';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chaiEnzyme from 'chai-enzyme';

chai.use(chaiEnzyme());
chai.use(sinonChai);

describe('SortComponent', () => {
  describe('render', () => {
    let wrapper, props, onSortSpy;
    beforeEach(() => {
      onSortSpy = sinon.spy();
      props = {
        sortableProperties: [{value:"Material Code",key:"material_code"}, {value:"Material Name",key:"material_name"}],
        onSort: onSortSpy,
        sortColumn: 'material_name',
        sortOrder: 'Ascending',
      };
      wrapper = shallow(<SortComponent {...props}/>);
    });
    it('should render Button with icon', () => {
      expect(wrapper.find('button')).to.have.length(1);
      const icon = wrapper.find('button Icon');
      expect(icon).to.have.length(1);
      expect(icon).to.have.prop('name');
      expect(wrapper.find('button')).to.include.text('Material Name');
    });

    context('when button is clicked', () => {
      let preventDefaultSpy;
      beforeEach(() => {
        preventDefaultSpy = sinon.spy();
        wrapper.find('button').simulate('click', {preventDefault: preventDefaultSpy});
      });
      it('should render sort properties', () => {
        expect(preventDefaultSpy).to.have.been.called;
        expect(wrapper.find('ul li button')).to.have.length(2);
      });
      it('should call onSort with selected sort property and direction', () => {
        wrapper.find('ul li button').first().simulate('click');
        expect(onSortSpy).to.have.been.calledWith('material_code', 'Ascending');
        expect(wrapper).to.not.have.descendants('ul');
      });
      it('should call onSort with changed Direction when sortColumn is same', () => {
        wrapper.find('ul li button').at(1).simulate('click');
        expect(onSortSpy).to.have.been.calledWith('material_name', 'Descending');
        expect(wrapper).to.not.have.descendants('ul');
      });
      it('should collapse when button is clicked again', () => {
        wrapper.find('button').first().simulate('click', {preventDefault: preventDefaultSpy});
        expect(wrapper).to.not.have.descendants('ul');
      });
    });
  });
});
