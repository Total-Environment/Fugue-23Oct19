import React from 'react';
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import chai, { expect } from 'chai';
import { shallow } from 'enzyme';
import chaiEnzyme from 'chai-enzyme';
import { CompositeDetails } from '../../../src/components/composite-details/index';


chai.use(chaiEnzyme());
chai.use(sinonChai);


describe('CompositeDetails', () => {
  let fetchDependencyDefinitionSpy, fetchMasterDataByNameSpy, fetchSfgDefinitionSpy;
  let props;
  beforeEach(() => {
    fetchDependencyDefinitionSpy = sinon.spy();
    fetchMasterDataByNameSpy = sinon.spy();
    fetchSfgDefinitionSpy = sinon.spy();
    props = {
      fetchDependencyDefinition: fetchDependencyDefinitionSpy,
      fetchMasterDataByName: fetchMasterDataByNameSpy,
      fetchSfgDefinition: fetchSfgDefinitionSpy,
      mode: 'create',
      componentType: 'sfg',
      statusData: {isFetching: false, values: [1,2]}
    };
  });

  it('should call dependency definition & master data on componentWillMount', () => {
    props.statusData = undefined;
    const wrapper = shallow(<CompositeDetails  {...props} />);
    expect(fetchDependencyDefinitionSpy).to.be.calledWith('material');
    expect(fetchDependencyDefinitionSpy).to.be.calledWith('service');
    expect(fetchMasterDataByNameSpy).to.be.calledWith('status');
    expect(fetchSfgDefinitionSpy).to.be.called;
  });

  it('should not reload sfg definition once loaded', () => {
    let newProps = Object.assign({}, props, {
      definition: {
        isFetching: false
      }
    });
    const wrapper = shallow(<CompositeDetails {...newProps} />);
    expect(fetchSfgDefinitionSpy).to.not.be.called;
  });

  it('should render CompositeComposition & set as active collapse panel on choosing a new component', () => {
    const wrapper = shallow(<CompositeDetails {...props} />);
    const SearchComponent = wrapper.find('SearchComponent');
    expect(wrapper.find('Collapse').prop('activeKey')).to.equal('sfg-composition');
    expect(wrapper.find('CompositeComposition')).to.not.exist;
    SearchComponent.simulate('chooseComponents', [{
      headers: [
        {
          columns:[],
          name: 'General',
          key: 'general'
        }
      ],
      id: 'test',
      type: 'material'
    }]);
    expect(wrapper.find('Collapse').prop('activeKey')).to.equal('sfg-composition');
    expect(wrapper.find('CompositeComposition')).to.exist;
  });

  it('should render CreateHeaderColumn component when there are no primary composition details', () => {
    const newProps = Object.assign({}, props, {
      definition: {
        isFetching: false
      },
      classificationData: {
        sfgClassifications: {
          isFetching: false
        }
      }
    });
    const wrapper = shallow(<CompositeDetails {...newProps} />);
    const SearchComponent = wrapper.find('SearchComponent');
    expect(wrapper.find('CreateHeaderColumn')).to.not.exist;
    SearchComponent.simulate('chooseComponents', [{
      headers: [
        {
          columns:[],
          name: 'General',
          key: 'general'
        }
      ],
      id: 'test',
      isPrimary: false,
      type: 'material'
    }]);
    expect(wrapper.update().find('CreateHeaderColumn')).to.exist;
  });

  it('should remove composition detail from the state by resource id', () => {
    const wrapper = shallow(<CompositeDetails {...props} />);
    const SearchComponent = wrapper.find('SearchComponent');
    const instance = wrapper.instance();
    const compositionDetail = {
      headers: [
        {
          columns:[],
          name: 'General',
          key: 'general'
        }
      ],
      id: 'test',
      isPrimary: false,
      type: 'material'
    };
    SearchComponent.simulate('chooseComponents', [compositionDetail]);
    expect(instance.state.compositionDetails).to.have.length(1);
    const CompositeComposition = wrapper.find('CompositeComposition');
    CompositeComposition.simulate('removeCompositionItem', compositionDetail.id);
    expect(instance.state.compositionDetails).to.have.length(0);
  });

  it('should reset Collapse active key to "sfg-composition", when the composition list is empty', () => {
    const wrapper = shallow(<CompositeDetails {...props} />);
    const SearchComponent = wrapper.find('SearchComponent');
    expect(wrapper.find('Collapse').prop('activeKey')).to.equal('sfg-composition');
    const compositionDetail = {
      headers: [
        {
          columns:[],
          name: 'General',
          key: 'general'
        }
      ],
      id: 'test',
      isPrimary: false,
      type: 'material'
    };
    SearchComponent.simulate('chooseComponents', [compositionDetail]);
    expect(wrapper.find('Collapse').prop('activeKey')).to.equal('sfg-composition');
    const CompositeComposition = wrapper.find('CompositeComposition');
    CompositeComposition.simulate('removeCompositionItem', compositionDetail.id);
    expect(wrapper.find('Collapse').prop('activeKey')).to.equal('sfg-composition');
  });
});
