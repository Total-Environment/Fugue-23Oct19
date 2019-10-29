import React from 'react';
import chai, {expect} from 'chai';
import {mount,shallow} from 'enzyme';
import {PriceBookCreate} from "../../../src/components/pricebook-create/index";
import {PriceBookFilters} from "../../../src/components/price-book/filters";
import sinon from 'sinon';
import sinonChai from 'sinon-chai';
import { idFor,tomorrowInIST } from '../../../src/helpers';
import moment from "moment";
import * as ReactDOM from "enzyme";


chai.use(sinonChai);

describe('Pricebook_create', () => {
  let props,wrapper;
  beforeEach(() => {
    props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy()
    };
    wrapper =new  shallow(<PriceBookCreate {...props}/>);
  });

  it('should call fetchDependencyDefinition when classificationData is not present', () => {
    props.fetchDependencyDefinition.should.have.been.called;
    props.onFetchProjects.should.have.been.called;
  });

  it('should set enteredLevelValues state when accumulateResults is called with key  level_values', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy()
    };
    let wrapper =new  PriceBookCreate(props);
    let levels = {level1:'primary',level2:'secondary'};
    wrapper.accumulateResults('level_values',levels);
    expect(wrapper.state.enteredLevelValues).to.be.equal(levels);
  });

  it('should set enteredLevelValues state when accumulateResults is called with key  cpr_values', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy()
    };
    let wrapper =new  PriceBookCreate(props);
    let cpr_values = {cpr1:0,cpr2:1};
    wrapper.accumulateResults('cpr_values',cpr_values);
    expect(wrapper.state.enteredCprValues).to.be.equal(cpr_values);
  });


  it('should set enteredComponentCode state when onResourceTypeChange is called with key  component_code', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);
    let component_code ='ALM0001';
    wrapper.instance().onResourceTypeChange('component_code',component_code);
    expect(wrapper.instance().state.enteredComponentCode).to.be.equal(component_code);
  });

  it('should set enteredProjectCode state when onResourceTypeChange is called with key  projectCode', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);
    let project_code ='POARR';
    wrapper.instance().onResourceTypeChange('projectCode',project_code);
    expect(wrapper.instance().state.enteredProjectCode).to.be.equal(project_code);
  });

  it('should set enteredProjectCode state to null when onResourceTypeChange is called with key  projectCode and val empty', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);
    let project_code ='';
    wrapper.instance().onResourceTypeChange('projectCode',project_code);
    expect(wrapper.instance().state.enteredProjectCode).to.be.equal(null);
  });

  it('should set enteredProjectCode,activeResourceType,enteredLevelValues states when onResourceTypeChange is called with key  resource_type', () => {
    let props = {
      classificationData: {MaterialClassifications:{isFetching:false,values:null}},
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);
    let resource_type ='Material';
    wrapper.instance().onResourceTypeChange('resource_type',resource_type);
    expect(wrapper.instance().state.activeResourceType).to.be.equal(resource_type);
    expect(wrapper.instance().state.enteredComponentCode).to.be.equal(null);
    expect(wrapper.instance().state.enteredLevelValues).to.be.equal(null);
    props.onDestroyComponentDetails.should.have.been.called;
  });

  it('should call resetState when onResourceTypeChange is called with key  resource_type and value empty', () => {
    let props = {
      classificationData: {MaterialClassifications:{isFetching:false,values:null}},
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);
    let resetState = sinon.spy(wrapper.instance(),"resetState");
    let resource_type ="";
    wrapper.instance().onResourceTypeChange('resource_type',resource_type);
    resetState.should.have.been.called;
  });

  it('should call resetState when onResourceTypeChange is called with key resource_type and value null', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);
    let resetState = sinon.spy(wrapper.instance(),"resetState");
    wrapper.instance().onResourceTypeChange('resource_type',null);
    resetState.should.have.been.called;
  });

  it('should reset all states when resetState is called', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy()
    };
    const wrapper  = mount(<PriceBookCreate {...props} />);

    let cprValues = {cpr_coefficient1:{value:"0",isValid:true,msg:''},
      cpr_coefficient2:{value:"0",isValid:true,msg:''},
      cpr_coefficient3:{value:"0",isValid:true,msg:''},
      cpr_coefficient4:{value:"0",isValid:true,msg:''},
      cpr_coefficient5:{value:"0",isValid:true,msg:''},
      cpr_coefficient6:{value:"0",isValid:true,msg:''},
      cpr_coefficient7:{value:"0",isValid:true,msg:''},
      cpr_coefficient8:{value:"0",isValid:true,msg:''},
    };
    wrapper.instance().resetState();
    expect(wrapper.instance().state.activeResourceType).to.be.equal(null);
    expect(wrapper.instance().state.selectedToEnterCode).to.be.equal(false);
    expect(wrapper.instance().state.selectedClassificationDropDown).to.be.equal(true);
    expect(wrapper.instance().state.enteredComponentCode).to.be.equal(null);
    expect(wrapper.instance().state.enteredLevelValues).to.be.equal(null);
    expect(wrapper.instance().state.enteredCprValues).to.be.deep.equal(cprValues);
    expect(wrapper.instance().state.selectedDate).to.be.equal(tomorrowInIST(new Date()));
    expect(wrapper.instance().state.enteredProjectCode).to.be.equal(null);
    expect(wrapper.instance().state.isComponentOpen).to.be.equal(true);
  });

  it('should render descendant modal', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy()
    };
    const wrapper = mount(<PriceBookCreate {...props} />);
    expect(wrapper.find('Modal')).to.have.length(1);
  });

  it('should close modal when close button is clicked', () => {
    let props = {
      classificationData: null,
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy(),
      onCloseModal: sinon.spy()
    };
    const wrapper = shallow(<PriceBookCreate {...props} />);
    wrapper.find('button').simulate('click',{preventDefault: () => {}});
    props.onCloseModal.should.have.been.called;
    expect(wrapper.instance().state.isComponentOpen).to.be.equal(false);
  });


  it('should render enabled dropdown and disabled component code input when dropdown is selected', () => {
    let props = {
      classificationData: {materialClassifications:{isFetching:false,values:null}},
      fetchDependencyDefinition: sinon.spy(),
      onFetchProjects: sinon.spy(),
      onDestroyComponentDetails: sinon.spy(),
      onCloseModal: sinon.spy()
    };
    const wrapper = shallow(<PriceBookCreate {...props} />);
    wrapper.setState({activeResourceType: 'Material'});
    // console.log(wrapper.instance().state.activeResourceType);
    expect(wrapper.find('input').at(0).prop('checked')).to.be.true;
    expect(wrapper.find('input').at(1).prop('checked')).to.be.false;
    expect(wrapper.find('PriceBookComponent')).to.have.length(1);
  });



});
