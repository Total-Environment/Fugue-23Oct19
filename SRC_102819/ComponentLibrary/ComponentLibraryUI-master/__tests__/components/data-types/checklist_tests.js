import React from 'react';
import {shallow} from 'enzyme';
import chai, {expect} from 'chai';
import chaiEnzyme from 'chai-enzyme';
import {CheckList, __RewireAPI__ as CheckListRewired} from '../../../src/components/data-types/check-list';

chai.use(chaiEnzyme());

describe('CheckList', () => {
  it('should render column value as link', () => {
    const props = {
      columnValue: {
        dataType: {
          name: "CheckList",
          subType: ""
        },
        value: {
          ui: "http://localhost:8080/check-lists/MISQ0001",
          data: "/check-lists/MFFQ0005",
          id: "MFFQ0005"
        }
      }
    };
    const wrapper = shallow(<CheckList {...props}/>);
    expect(wrapper).to.have.descendants('a');
    expect(wrapper).to.have.prop('href', "/check-lists/MFFQ0005");
  });

  it('should render input when it is editable and value is null', () => {
    const props = {
      columnValue: {
        value: null,
        dataType: {
          name: "CheckList",
          subType: ""
        },
        editable: true
      }
    };
    const wrapper = shallow(<CheckList {...props}/>);
    expect(wrapper).to.have.descendants('button');
  });

  it('should render remove button if props do not have hide remove and value is not present', () => {
    const props = {
      columnValue: {
        value: null,
        dataType: {
          name: "CheckList",
          subType: ""
        },
        editable: true
      }
    };
    CheckListRewired.__Rewire__('styles', {delete: 'delete'});
    const wrapper = shallow(<CheckList {...props}/>);
    expect(wrapper).to.have.descendants('button');
    expect(wrapper.find('button').at(1).prop('className')).to.equal('delete');
  });

  it('should not render remove button if props have hide remove and value as null', () => {
    const props = {
      columnValue: {
        value: null,
        dataType: {
          name: "CheckList",
          subType: ""
        },
        editable: true
      }, hideRemove: true
    };
    CheckListRewired.__Rewire__('styles', {delete: 'delete'});
    const wrapper = shallow(<CheckList {...props}/>);
    expect(wrapper).to.have.exactly(1).descendants('button');
  });

  it('should render link file and not render choose file button if props have column value and its data', () => {
    const props = {
      columnValue: {
        value: {name: "MG0001", data: "tw.com/MGP001", id: 'MGP001'},
        dataType: {
          name: "CheckList",
          subType: ""
        },
        editable: true
      }, hideRemove: true
    };
    const wrapper = shallow(<CheckList {...props}/>);
    expect(wrapper).to.have.descendants('a');
    expect(wrapper.find('a')).to.have.prop('href', '/check-lists/MGP001');
    expect(wrapper).to.not.have.descendants('button');
  });

  it('should not render link and render choose file button file if props have column value and its data', () => {
    const props = {
      columnValue: {
        value: null,
        dataType: {
          name: "CheckList",
          subType: ""
        },
        editable: true
      }, hideRemove: true
    };
    const wrapper = shallow(<CheckList {...props}/>);
    expect(wrapper).to.not.have.descendants('a');
    expect(wrapper).to.have.descendants('button');
  });
});
